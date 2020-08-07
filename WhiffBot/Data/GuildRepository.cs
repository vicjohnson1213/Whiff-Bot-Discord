using Discord.WebSocket;
using Npgsql;
using System.Collections.Generic;
using WhiffBot.Configuration;
using WhiffBot.Models;

namespace WhiffBot.Data
{
    public class GuildRepository : IGuildRepository
    {
        private string ConnectionString { get; set; }

        private readonly string INIT_GUILD = "INSERT INTO guild VALUES (@guildId, @guildName)";
        private readonly string INIT_GUILD_SETTINGS = "INSERT INTO guild_settings VALUES (@guildId)";

        private readonly string GET_GUILD = "SELECT * FROM guild g WHERE g.id = @id LIMIT 1";
        private readonly string GET_GUILD_SETTINGS = "SELECT * FROM guild_settings gs WHERE gs.guild_id = @id LIMIT 1";
        private readonly string GET_GUILD_ROLE_ASSIGNMENT_ROLES = "SELECT * FROM guild_role_assignment gra WHERE gra.guild_id = @id";

        private readonly string SAVE_GUILD_ROLE_ASSIGNMENT = "UPDATE guild_settings gs SET role_assignment_channel_id = @channelId, role_assignment_message_id = @messageId where guild_id = @guildId";
        private readonly string ADD_ASSIGNABLE_ROLE = "INSERT INTO guild_role_assignment VALUES (@guildId, @reaction, @roleId)";

        private readonly string REMOVE_ASSIGNABLE_ROLE = "DELETE FROM guild_role_assignment WHERE guild_id = @guildId AND role_id = @roleId";

        public GuildRepository(IConfiguration config)
        {
            ConnectionString = config.ConnectionString;
        }

        public void InitGuild(SocketGuild guild)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(INIT_GUILD, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guild.Id);
                    cmd.Parameters.AddWithValue("guildname", guild.Name);
                    var reader = cmd.ExecuteNonQuery();
                }

                conn.Close();
            }

            InitGuildSettings(guild);
        }

        private void InitGuildSettings(SocketGuild guild)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(INIT_GUILD_SETTINGS, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guild.Id);
                    var reader = cmd.ExecuteReader();
                }

                conn.Close();
            }
        }

        /// <summary>
        /// Gets all available data for a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild requested</param>
        /// <returns>The guild information for this guild</returns>
        public Guild Get(ulong guildId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(GET_GUILD, conn))
                {
                    cmd.Parameters.AddWithValue("id", (long)guildId);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return new Guild
                        {
                            Id = (ulong)reader.GetInt64(0),
                            Name = reader.GetString(1),
                            Settings = GetSettings(guildId)
                        };
                    }
                }

                conn.Close();
            }

            return null;
        }

        /// <summary>
        /// Gets the guild settings for a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild requested</param>
        /// <returns>The guild settings for this guild</returns>
        public GuildSettings GetSettings(ulong guildId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(GET_GUILD_SETTINGS, conn))
                {
                    cmd.Parameters.AddWithValue("id", (long)guildId);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return new GuildSettings
                        {
                            Prefix = reader.GetChar(1),
                            AuditChannelId = (ulong?)reader.GetFieldValue<long?>(2),
                            RoleAssignment = new RoleAssignmentSettings
                            {
                                ChannelId = (ulong?)reader.GetFieldValue<long?>(3),
                                MessageId = (ulong?)reader.GetFieldValue<long?>(4),
                                Roles = GetRoleAssignmentRoles(guildId)
                            }
                        };
                    }
                }

                conn.Close();
            }

            return null;
        }

        /// <summary>
        /// Saves the role assignment details for a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild to save the settings for</param>
        /// <param name="channelId">The role assignment channel</param>
        /// <param name="messageId">The role assignment message</param>
        public void SaveRoleAssignmentSettings(ulong guildId, ulong channelId, ulong messageId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SAVE_GUILD_ROLE_ASSIGNMENT, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guildId);
                    cmd.Parameters.AddWithValue("channelId", (long)channelId);
                    cmd.Parameters.AddWithValue("messageId", (long)messageId);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        /// <summary>
        /// Saves a new assignable role for a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild to add a role to</param>
        /// <param name="reaction">The reaction assiciated to the assignable role</param>
        /// <param name="roleId">The role id associated with the assignable role</param>
        public void AddAssignableRole(ulong guildId, string reaction, ulong roleId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(ADD_ASSIGNABLE_ROLE, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guildId);
                    cmd.Parameters.AddWithValue("reaction", reaction);
                    cmd.Parameters.AddWithValue("roleId", (long)roleId);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        /// <summary>
        /// Removes an assignable role from a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild to remove the role</param>
        /// <param name="roleId">The role id of the role to remove</param>
        public void RemoveAssignableRole(ulong guildId, ulong roleId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(REMOVE_ASSIGNABLE_ROLE, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guildId);
                    cmd.Parameters.AddWithValue("roleId", (long)roleId);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        /// <summary>
        /// Gets all assignable roles for a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild to get roles for</param>
        /// <returns>All assignable roles for this guild</returns>
        private List<RoleAssignmentRole> GetRoleAssignmentRoles(ulong guildId)
        {
            var roles = new List<RoleAssignmentRole>();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(GET_GUILD_ROLE_ASSIGNMENT_ROLES, conn))
                {
                    cmd.Parameters.AddWithValue("id", (long)guildId);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        roles.Add(new RoleAssignmentRole
                        {
                            Reaction = reader.GetString(1),
                            RoleId = (ulong)reader.GetInt64(2)
                        }); ;
                    }
                }

                conn.Close();
            }

            return roles;
        }
    }
}
