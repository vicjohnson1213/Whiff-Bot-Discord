using Npgsql;
using System.Collections.Generic;
using WhiffBot.Model.Configuration;
using WhiffBot.Model;

namespace WhiffBot.Data
{
    public class GuildRepository : IGuildRepository
    {
        private string ConnectionString { get; set; }

        private readonly string INIT_GUILD = "INSERT INTO guild VALUES (@guildId, @guildName)";
        private readonly string INIT_GUILD_SETTINGS = "INSERT INTO guild_settings VALUES (@guildId)";

        private readonly string UPDATE_GUILD_NAME = "UPDATE guild g SET name = @guildName where id = @guildId";

        private readonly string GET_GUILDS = "SELECT * FROM guild g";
        private readonly string GET_GUILD = "SELECT * FROM guild g WHERE g.id = @id LIMIT 1";
        private readonly string GET_GUILD_SETTINGS = "SELECT * FROM guild_settings gs WHERE gs.guild_id = @id LIMIT 1";
        private readonly string GET_GUILD_ROLE_ASSIGNMENT_ROLES = "SELECT * FROM guild_role_assignment gra WHERE gra.guild_id = @id";

        private readonly string GET_GUILD_AUTO_RESPONSES = "SELECT * FROM guild_responses gr WHERE gr.guild_id = @id";
        private readonly string ADD_AUTO_RESPONSE = "INSERT INTO guild_responses VALUES (@guildId, @message, @response)";
        private readonly string REMOVE_AUTO_RESPONSE = "DELETE FROM guild_responses WHERE guild_id = @guildId AND message = @message";

        private readonly string SAVE_GUILD_ROLE_ASSIGNMENT = "UPDATE guild_settings gs SET role_assignment_channel_id = @channelId, role_assignment_message_id = @messageId where guild_id = @guildId";
        private readonly string SET_AUDIT_LOG_CHANNEL = "UPDATE guild_settings SET audit_log_channel_id = @channelId WHERE guild_id = @guildId";
        private readonly string ADD_ASSIGNABLE_ROLE = "INSERT INTO guild_role_assignment VALUES (@guildId, @reaction, @roleId)";

        private readonly string REMOVE_ASSIGNABLE_ROLE = "DELETE FROM guild_role_assignment WHERE guild_id = @guildId AND role_id = @roleId";

        public GuildRepository(IConfiguration config)
        {
            ConnectionString = config.ConnectionString;
        }

        public void InitGuild(Guild guild)
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

        public List<Guild> GetJoinedGuilds()
        {
            var guilds = new List<Guild>();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(GET_GUILDS, conn))
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                        guilds.Add(new Guild
                        {
                            Id = (ulong)reader.GetInt64(0),
                            Name = reader.GetString(1)
                        });
                }
            }

            return guilds;
        }

        /// <summary>
        /// Gets all available data for a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild requested</param>
        /// <returns>The guild information for this guild</returns>
        public Guild Get(ulong guildId)
        {
            Guild guild = null;

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(GET_GUILD, conn))
                {
                    cmd.Parameters.AddWithValue("id", (long)guildId);
                    var reader = cmd.ExecuteReader();

                    // Take the first result if any are present
                    if (reader.Read())
                    {
                        guild = new Guild
                        {
                            Id = (ulong)reader.GetInt64(0),
                            Name = reader.GetString(1),
                            Settings = GetSettings(guildId),
                            AutoResponses = GetAutoResponses(guildId)
                        };
                    }
                }

                conn.Close();
            }

            return guild;
        }

        /// <summary>
        /// Updates the guild name stored in the database
        /// </summary>
        /// <param name="guildId">The guild id of the guild to update</param>
        /// <param name="guildName">The new name of the guild</param>
        public void UpdateGuildName(ulong guildId, string guildName)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(UPDATE_GUILD_NAME, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guildId);
                    cmd.Parameters.AddWithValue("guildName", guildName);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        /// <summary>
        /// Gets the guild settings for a guild.
        /// </summary>
        /// <param name="guildId">The guild id of the guild requested</param>
        /// <returns>The guild settings for this guild</returns>
        public GuildSettings GetSettings(ulong guildId)
        {
            GuildSettings settings = null;

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(GET_GUILD_SETTINGS, conn))
                {
                    cmd.Parameters.AddWithValue("id", (long)guildId);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        settings = new GuildSettings
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

            return settings;
        }

        public void AddAutoResponse(ulong guildId, string message, string response)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(ADD_AUTO_RESPONSE, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guildId);
                    cmd.Parameters.AddWithValue("message", message);
                    cmd.Parameters.AddWithValue("response", response);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void RemoveAutoResponse(ulong guildId, string message)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(REMOVE_AUTO_RESPONSE, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guildId);
                    cmd.Parameters.AddWithValue("message", message);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
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

        public void SetAuditLogChannel(ulong guildId, ulong channelId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SET_AUDIT_LOG_CHANNEL, conn))
                {
                    cmd.Parameters.AddWithValue("guildId", (long)guildId);
                    cmd.Parameters.AddWithValue("channelId", (long)channelId);
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
        /// Gets the autoresponses for a sesrver
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAutoResponses(ulong guildId)
        {
            var responses = new Dictionary<string, string>();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(GET_GUILD_AUTO_RESPONSES, conn))
                {
                    cmd.Parameters.AddWithValue("id", (long)guildId);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        responses.Add(reader.GetString(1), reader.GetString(2));
                    }
                }

                conn.Close();
            }

            return responses;
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
                            Reaction = reader.GetString(1).Trim(),
                            RoleId = (ulong)reader.GetInt64(2)
                        }); ;
                    }
                }

                conn.Close();
            }

            return roles;
        }

        private void InitGuildSettings(Guild guild)
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
    }
}
