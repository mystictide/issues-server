using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Helpers;
using issues.server.Infrastructure.Data.Managers.Auth;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Repo.Main
{
    public class UserRepository : AppSettings, IUsers
    {
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
                var access = await new AuthManager().CheckEmail(true, Email, ID);
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
    }
}