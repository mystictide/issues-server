using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Data.Interface;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Helpers;

namespace issues.server.Infrastructure.Data.Repo.Main
{
    public class RolesRepository : AppSettings, IBase<Roles>
    {
        public async Task<FilteredList<Roles>?> FilteredList(Filter filter)
        {
            try
            {
                var filterModel = new Roles();
                FilteredList<Roles> request = new FilteredList<Roles>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Roles> result = new FilteredList<Roles>();
                string kw = "''";
                if (filter.Keyword != null)
                {
                    kw = $@"'%{filter.Keyword}%'";
                }

                string WhereClause = $@"WHERE t.name ilike '%{filter.Keyword}%'";
                string query_count = $@"Select Count(t.id) from roles t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *
                    FROM roles t
                    {WhereClause}
                    order by id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Roles>(query);
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
        public async Task<Roles?> Get(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM roles t
                WHERE t.id = {ID};";
                string attrQuery = $@"
                SELECT t.attributeid 
                FROM roleattributes t
                WHERE t.roleid = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Roles>(query);
                        res.Attributes = (List<int>)await con.QueryAsync<int>(attrQuery);
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
        public async Task<Roles?> Manage(Roles entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";

                if (entity.Name.Contains("'"))
                {
                    entity.Name = entity.Name.Replace("'", "''");
                }

                string query = $@"
                INSERT INTO roles (id, companyid, name, isactive)
	 	        VALUES ({identity}, {entity.CompanyID}, '{entity.Name}', true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}'
                RETURNING *;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Roles>(query);
                    query = $@"DELETE from roleattributes where roleid = {res.ID};";
                    await connection.QueryFirstOrDefaultAsync<int>(query);
                    var atts = new List<int>();
                    foreach (var item in entity.Attributes)
                    {
                        query = $@"
                        INSERT INTO roleattributes (id, roleid, attributeid)
	 	                VALUES (default, {res.ID}, {item})
                        RETURNING attributeid;";
                        int id = await connection.QueryFirstOrDefaultAsync<int>(query);
                        atts.Add(id);
                    }
                    res.Attributes = atts;
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<bool> Archive(Roles entity)
        {
            try
            {
                string query = $@"
                UPDATE roles
                SET isactive = {entity.IsActive} 
                WHERE id = {entity.ID} 
                RETURNING isactive;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
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
