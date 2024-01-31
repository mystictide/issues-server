using Dapper;
using issues.server.Infrasructure.Models.Users;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Helpers;
using issues.server.Infrastructure.Data.Interface.User;
using issues.server.Infrasructure.Models.Users.Helpers;

namespace issues.server.Infrastructure.Data.Repo.User
{
    public class UserRepository : AppSettings, IUsers
    {
        public async Task<Users>? Register(Users entity)
        {
            ProcessResult result = new ProcessResult();
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Email", entity.Email);
                param.Add("@Username", entity.Username);
                param.Add("@Password", entity.Password);

                string query = $@"
                INSERT INTO users (email, username, password, authtype, isactive)
	                VALUES (@Email, @Username, @Password, 1, true)
                RETURNING *;";
                //string usQuery = $@"
                //INSERT INTO usersettings (userid, picture, bio, facebook, instagram, linkedin, twitter, youtube, personal)
	               // VALUES (@UserID, null, null, null, null, null, null, null, null)";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query, param);
                    //param.Add("@UserID", res?.ID);
                    //await con.QueryFirstOrDefaultAsync<Users>(usQuery, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.State = ProcessState.Error;
                return null;
            }
        }

        public async Task<Users>? Login(Users entity)
        {
            try
            {
                string WhereClause = $"WHERE (t.email = '{entity.Email}');";

                string query = $@"
                SELECT *
                FROM users t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<bool> CheckEmail(string Email, int? UserID)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserID", UserID);
            param.Add("@Email", Email);

            string Query;
            if (UserID.HasValue)
            {
                Query = @"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE email = @Email AND NOT (id = @UserID);";
            }
            else
            {
                Query = @"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE email = @Email;";
            }

            using (var con = GetConnection)
            {
                var res = await con.QueryAsync<bool>(Query, param);
                return res.FirstOrDefault();
            }
        }

        public async Task<bool> CheckUsername(string Username, int? UserID)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserID", UserID);
            param.Add("@Username", Username);

            string Query;
            if (UserID.HasValue)
            {
                Query = @"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE username = @Username AND NOT (id = @UserID);";
            }
            else
            {
                Query = @"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE username = @Username;";
            }

            using (var con = GetConnection)
            {
                var res = await con.QueryAsync<bool>(Query, param);
                return res.FirstOrDefault();
            }
        }

        public async Task<Users>? Get(int? ID, string? Username)
        {
            try
            {
                string WhereClause = $" WHERE t.id = {ID ?? 0}  OR (t.username = '{Username}')";

                string query = $@"
                SELECT *
                FROM users t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<string>? UpdateEmail(int ID, string Email)
        {
            try
            {
                var access = await CheckEmail(Email, ID);
                if (!access)
                {
                    string query = $@"
                    UPDATE users
                    SET email = '{Email}'
                    WHERE id = {ID}
                    RETURNING email;";
                    using (var connection = GetConnection)
                    {
                        var res = await connection.QueryFirstOrDefaultAsync<string>(query);
                        return res;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<bool>? ChangePassword(int UserID, string currentPassword, string newPassword)
        {
            try
            {
                string query = $@"
                UPDATE users
                SET password = '{newPassword}'
                WHERE id = {UserID};";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync(query);
                    return true;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<bool>? UpdateUsername(int ID, string Username)
        {
            try
            {
                string query = $@"
                UPDATE users
                SET username = '{Username}'
                WHERE id = {ID};";

                using (var connection = GetConnection)
                {
                    await connection.QueryFirstOrDefaultAsync<ProcessResult>(query);
                    return true;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<string>? UpdateBio(int ID, string Bio)
        {
            try
            {
                string query = $@"
                UPDATE usersettings
                SET bio = '{Bio}'
                WHERE userid = {ID}
                RETURNING bio;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<string>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<bool?> ManageFollow(int TargetID, int UserID)
        {
            try
            {
                var ID = await GetUserFunctionID(TargetID, UserID, true);
                string query = "";
                if (!ID.HasValue)
                {
                    query = $@"
                INSERT INTO userfollowjunction (id, followerid, followedid)
	 	                VALUES (default, {UserID}, {TargetID})
                ON CONFLICT (id, followerid, followedid) DO NOTHING 
                RETURNING True;";
                }
                else
                {
                    query = $@"delete from userfollowjunction where followerid = {UserID} and followedid = {TargetID} RETURNING False;";
                }

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<bool?> ManageBlock(int TargetID, int UserID)
        {
            try
            {
                var ID = await GetUserFunctionID(TargetID, UserID, false);
                string query = "";
                if (!ID.HasValue)
                {
                    query = $@"
                INSERT INTO userblockjunction (id, blockerid, blockedid)
	 	                VALUES (default, {UserID}, {TargetID})
                ON CONFLICT (id, blockerid, blockedid) DO NOTHING 
                RETURNING True;";
                }
                else
                {
                    query = $@"delete from userblockjunction where blockerid = {UserID} and blockedid = {TargetID} RETURNING False;";
                }

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<int?> GetUserFunctionID(int TargetID, int UserID, bool function)
        {
            try
            {
                string query = "";
                if (function)
                {
                    query = $@"Select id from userfollowjunction ufj where ufj.followerid = {UserID} and ufj.followedid = {TargetID};";
                }
                else
                {
                    query = $@"Select id from userblockjunction ubj where ubj.blockerid = {UserID} and ubj.blockedid = {TargetID};";
                }

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<int?>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
    }
}