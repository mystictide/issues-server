using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Helpers;
using issues.server.Infrasructure.Models.Users.Helpers;
using issues.server.Infrastructure.Data.Interface.Auth;

namespace issues.server.Infrastructure.Data.Repo.Auth
{
    public class AuthRepository : AppSettings, IAuth
    {
        public async Task<Companies>? Register(Companies entity)
        {
            ProcessResult result = new ProcessResult();
            try
            {
                string query = $@"
                INSERT INTO companies (email, name, password)
	                VALUES ('{entity.Email}', '{entity.Name}', '{entity.Password}')
                RETURNING *;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Companies>(query);
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
        public async Task<Companies>? CompanyLogin(Companies entity)
        {
            try
            {
                string WhereClause = $"WHERE (t.email = '{entity.Email}');";

                string query = $@"
                SELECT *
                FROM companies t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Companies>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<Users>? Login(Users entity)
        {
            try
            {
                string WhereClause = $"WHERE (t.email = '{entity.Email}');";

                string query = $@"
                SELECT t.*, c.id, c.name, r.*
                FROM users t
                left join companies c on c.id = t.companyid
                left join roles r on r.id = t.roleid
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<Users, Companies, Roles, Users>(query, (i, u, r) =>
                    {
                        i.Company = u ?? new Companies();
                        i.Role = r ?? new Roles();
                        return i;
                    }, splitOn: "id");
                    var result = res.FirstOrDefault();
                    if (result != null && result.ID > 0)
                    {
                        query = $@"
                        SELECT t.attributeid
                        FROM roleattributes t
                        where t.roleid = {result.Role.ID};";
                        var attrs = await con.QueryAsync<int>(query);
                        result.Role.Attributes = attrs.ToList();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<bool> CheckEmail(bool company, string Email, int? UserID)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserID", UserID);
            param.Add("@Email", Email);

            string Query;
            if (UserID.HasValue)
            {
                Query = $@"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM {(company ? "companies" : "users")} 
                WHERE email = @Email AND NOT (id = @UserID);";
            }
            else
            {
                Query = $@"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM {(company ? "companies" : "users")} 
                WHERE email = @Email;";
            }

            using (var con = GetConnection)
            {
                var res = await con.QueryAsync<bool>(Query, param);
                return res.FirstOrDefault();
            }
        }
    }
}
