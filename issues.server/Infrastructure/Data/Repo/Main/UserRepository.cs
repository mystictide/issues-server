using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Helpers;
using issues.server.Infrastructure.Data.Managers.Auth;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Repo.Main
{
    public class UserRepository : AppSettings, IUsers
    {
        public async Task<Users?> Manage(Users entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";

                if (entity.FirstName.Contains("'"))
                {
                    entity.FirstName = entity.FirstName.Replace("'", "''");
                }
                if (entity.LastName.Contains("'"))
                {
                    entity.LastName = entity.LastName.Replace("'", "''");
                }

                string query = $@"
                INSERT INTO users (id, companyid, firstname, lastname, email, password, roleid, isactive)
	 	        VALUES ({identity}, {entity.Company.ID}, '{entity.FirstName}', '{entity.LastName}', '{entity.Email}', '{entity.Password}', {entity.Role.ID}, true)
                ON CONFLICT (id) DO UPDATE 
                SET firstname = '{entity.FirstName}',
                      lastname = '{entity.LastName}',
                      email = '{entity.Email}',
                      password = COALESCE('{entity.Password}', users.password),
                      roleid = {entity.Role.ID}
                RETURNING id;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Users>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<Users?> Get(int ID)
        {
            try
            {
                string WhereClause = $" WHERE t.id = {ID}";

                string query = $@"
                SELECT t.id, t.firstname, t.lastname, t.email, r.*, c.*
                FROM users t
                left join roles r on r.id = t.roleid
                left join companies c on c.id = t.companyid 
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<Users, Roles, Companies, Users>(query, (u, r, c) =>
                    {
                        u.Role = r;
                        u.Company = c;
                        return u;
                    }, splitOn: "id");
                    return res.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<string?> UpdateEmail(int ID, string Email)
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

        public async Task<bool?> ChangePassword(int UserID, string currentPassword, string newPassword)
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

        public async Task<FilteredList<Users>?> FilteredList(Filter filter)
        {
            try
            {
                var filterModel = new Users();
                FilteredList<Users> request = new FilteredList<Users>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Users> result = new FilteredList<Users>();
                string kw = "''";
                if (filter.Keyword != null)
                {
                    kw = $@"'%{filter.Keyword}%'";
                }

                string WhereClause = $@"WHERE t.companyid = {filter.CompanyID} and TRIM(CONCAT(t.firstname, ' ', t.lastname)) ilike '%{filter.Keyword}%'";
                string query_count = $@"Select Count(t.id) from users t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT t.id, t.firstname, t.lastname, t.email, r.*
                    FROM users t
                    left join roles r on r.id = t.roleid
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Users, Roles, Users>(query, (u, r) =>
                    {
                        u.Role = r;
                        return u;
                    }, splitOn: "id");
                    result.filter = request.filter;
                    result.filterModel = request.filterModel;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Users>?> GetCompanyUsers(int ID)
        {
            try
            {
                string query = $@"
                SELECT t.id, t.firstname, t.lastname, r.*
                FROM users t
                left join roles r on r.id = t.roleid
                WHERE t.companyid = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryAsync<Users, Roles, Users>(query, (u, r) =>
                        {
                            u.Role = r;
                            return u;
                        }, splitOn: "id");
                        return res;
                    }
                    return null;
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