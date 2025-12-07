using Backend_Cooking_Kid_DataAccess.ValidateConverts;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Cooking_Kid_DataAccess.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        //Base
        Task<List<dynamic>> GetAllAsync(string tableName, int? page, int? pageSize);
        Task<List<dynamic>> GetAllByIdAsync(string tableName, Dictionary<string, object>? pkValue, int? page, int? pageSize);
        Task<bool> UpdateAsync(string tableName, IEnumerable<Dictionary<string, object>> pkValues, IEnumerable<Dictionary<string, object>> initialDatasList);
        Task<dynamic?> GetByIdAsync(object id, string tableName, string? keyName = null);
        Task<int> UpsertAsync(DataTable table, ModelDefination sqlJsonDefination);

        //External
        DataTable CheckExcelColumnMapping(DataTable oldDataTable, ModelDefination.ExcelIntegrationMap excelColumn);

        //get Template
        Task<ModelDefination> GetTemplateModelAsync(string tableName);
        Task<List<ModelDefination>> GetAllTemplateModelAsync();
        Task<FormDefination> GetTemplateMetadataAsync(string controller);
    }
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly CookingKidContext _context;
        private readonly IDbConnection _dbConnect;
        private readonly DbSet<T> _dbSet;
        public BaseRepository(CookingKidContext context)
        {
            _context = context;
            _dbConnect = context.Database.GetDbConnection();
            _dbSet = _context.Set<T>();
        }
        #region Base
        public async Task<List<dynamic>> GetAllAsync(string tableName, int? page, int? pageSize)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 100;
            try
            {
                var connectionString = _context.Database.GetConnectionString(); // lấy từ EF Core
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                var sql = "";
                // Lấy tổng số dòng
                string countSql = $"SELECT COUNT(*) FROM [{tableName}]";
                int totalCount = await conn.ExecuteScalarAsync<int>(countSql);
                if (page != null && pageSize != null && page >= 0 && pageSize >= 0)
                {
                    // Lấy dữ liệu trang hiện tại
                    int offset = ((int)page - 1) * (int)pageSize;
                    sql = $@"
								SELECT * 
								FROM [{tableName}] 
								ORDER BY (SELECT NULL) -- Tránh lỗi nếu không có cột cụ thể
								OFFSET {offset} ROWS 
								FETCH NEXT {pageSize} ROWS ONLY";
                }
                else
                {
                    sql = $"SELECT * FROM [{tableName}]";
                }
                var items = await conn.QueryAsync(sql);
                return items.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception when fetching paged data from table '{tableName}'", ex);
            }
        }

        public async Task<List<dynamic>> GetAllByIdAsync(string tableName, Dictionary<string, object>? pkValue, int? page, int? pageSize)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));
            if (pkValue == null)
                throw new ArgumentNullException(nameof(pkValue));
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 100;
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                var parameters = new DynamicParameters();
                var whereConditions = new List<string>();

                foreach (var pk in pkValue)
                {
                    string paramName = "@" + pk.Key;
                    parameters.Add(paramName, pk.Value);
                    whereConditions.Add($"[{pk.Key}] = {paramName}");
                }

                string whereClause = whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                //string countSql = $"SELECT COUNT(*) FROM [{tableName}]{whereClause}";
                //int totalCount = await conn.ExecuteScalarAsync<int>(countSql, parameters);

                var sql = "";
                if (page != null && pageSize != null && page >= 0 && pageSize >= 0)
                {
                    int offset = ((int)page - 1) * (int)pageSize;
                    sql = $@"
                            SELECT *
                            FROM [{tableName}]
                            {whereClause}
                            ORDER BY (SELECT NULL)
                            OFFSET {offset} ROWS
                            FETCH NEXT {pageSize} ROWS ONLY";
                }
                else
                {
                    sql = $"SELECT * FROM [{tableName}]{whereClause}";
                }

                var items = await conn.QueryAsync(sql, parameters);

                return items.ToList();
            }
            catch (Exception ex)
            {
                throw new ExceptionFormat($"Get all by ID error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get by id of table
        /// </summary>
        /// <param name="id">value primarykey</param>
        /// <param name="tableName">table name</param>
        /// <param name="keyName">key name or null</param>
        /// <returns>List or T object</returns>
        /// <exception cref="ExceptionFormat"></exception>
        public async Task<dynamic?> GetByIdAsync(object id, string tableName, string? keyName = null)
        {
            if (id == null)
                throw new ExceptionFormat("Giá trị khoá chính là null");

            var sqlDef = await GetTemplateModelAsync(tableName);
            if (sqlDef == null)
                throw new ExceptionFormat($"Không tìm thấy định nghĩa bảng {tableName}");

            // Nếu id là Dictionary → dùng multi-key
            if (id is IDictionary<string, string> keyDict)
            {
                var whereClause = string.Join(" AND ", keyDict.Select(k => $"[{k.Key}] = @{k.Key}"));
                var sql = $"SELECT * FROM [{tableName}] WHERE {whereClause}";
                var parameters = new DynamicParameters();
                foreach (var kv in keyDict)
                {
                    parameters.Add(kv.Key, kv.Value);
                }
                if (_dbConnect.State != ConnectionState.Open)
                    _dbConnect.Open();
                var results = await _dbConnect.QueryAsync<dynamic>(sql, parameters);
                return results;
            }
            else if (id is string)
            {
                // Nếu là single key
                if (string.IsNullOrEmpty(keyName))
                {
                    keyName = GetPrimaryKeys(sqlDef).FirstOrDefault();
                    if (string.IsNullOrEmpty(keyName))
                        throw new ExceptionFormat("Không tìm thấy khóa chính");
                }

                var sqlSingle = $"SELECT * FROM [{tableName}] WHERE [{keyName}] = @id";

                if (_dbConnect.State != ConnectionState.Open)
                    _dbConnect.Open();
                var result = await _dbConnect.QueryAsync<dynamic>(sqlSingle, new { id });
                return result;
            }
            else
            {
                throw new ExceptionFormat("Dữ liệu id truyền vào không hợp lệ");
            }
        }

        public async Task<bool> UpdateAsync(
                                            string tableName,
                                            // pkValues là danh sách các Dictionary, mỗi Dictionary là PK của một hàng
                                            IEnumerable<Dictionary<string, object>> pkValues,
                                            // initialDatasList là danh sách các Dictionary, mỗi Dictionary là dữ liệu cần cập nhật của một hàng
                                            IEnumerable<Dictionary<string, object>> initialDatasList
                                            )
        {
            // Đảm bảo số lượng PK và số lượng dữ liệu khớp nhau
            if (pkValues.Count() != initialDatasList.Count())
            {
                throw new ArgumentException("The number of primary key sets must match the number of data sets.");
            }

            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                // Danh sách các tham số để Dapper thực thi theo lô
                var batchParameters = new List<DynamicParameters>();

                // Câu lệnh SQL (được xây dựng một lần)
                string sql = "";

                // Lặp qua từng bản ghi để xây dựng DynamicParameters
                for (int i = 0; i < pkValues.Count(); i++)
                {
                    var pk = pkValues.ElementAt(i);
                    var data = initialDatasList.ElementAt(i);

                    var parameters = new DynamicParameters();
                    var setClauses = new List<string>();
                    var whereClauses = new List<string>();

                    // Xây dựng SET clause
                    foreach (var item in data)
                    {
                        // KHÔNG cần thêm ký tự @ vì Dapper xử lý DynamicParameters
                        parameters.Add(item.Key, item.Value);
                        setClauses.Add($"[{item.Key}] = @{item.Key}");
                    }
                    if (setClauses.Count == 0) continue; // Bỏ qua nếu không có gì để cập nhật
                    string setClause = string.Join(", ", setClauses);

                    foreach (var item in pk)
                    {
                        string pkParamName = $"{item.Key}";
                        parameters.Add(pkParamName, item.Value);
                        whereClauses.Add($"[{item.Key}] = @{pkParamName}");
                    }
                    if (whereClauses.Count == 0) throw new ExceptionFormat("Not have where clause for one record");
                    string whereClause = " WHERE " + string.Join(" AND ", whereClauses);

                    // Gán SQL chỉ trong lần lặp đầu tiên (cần đảm bảo cấu trúc cột giống nhau)
                    if (string.IsNullOrEmpty(sql))
                    {
                        sql = $"UPDATE [{tableName}] SET {setClause}{whereClause}";
                    }

                    batchParameters.Add(parameters);
                }

                if (batchParameters.Count == 0) return true; // Không có gì để cập nhật

                // 💡 Dapper tự động thực thi theo lô khi truyền List<DynamicParameters>
                var rowsAffected = await conn.ExecuteAsync(sql, batchParameters);

                return rowsAffected == batchParameters.Count; // Trả về true nếu tất cả các hàng đều được cập nhật
            }
            catch (Exception ex)
            {
                throw new ExceptionFormat(ex.Message);
            }
        }


        /// <summary>
        /// Update or insert data to db
        /// </summary>
        /// <param name="table">value of datatable</param>
        /// <param name="sqlJsonDefination">value of jsonDefination</param>
        /// <returns>number row if success or list errors</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ExceptionFormat"></exception>
        public async Task<int> UpsertAsync(DataTable table, ModelDefination sqlJsonDefination)
        {
            if (sqlJsonDefination == null) throw new ArgumentNullException(nameof(sqlJsonDefination));
            var tableName = sqlJsonDefination.ExcelIntegration?.SheetName ?? sqlJsonDefination.Model;
            var tempTableName = $"#{tableName}_Temp";
            bool isRollbacked = false;

            using (var connection = new SqlConnection(_dbConnect.ConnectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //tạo bảng tạm 
                        var createTempSql = GenerateCreateTableScript(tempTableName, sqlJsonDefination);
                        using (var createCmd = new SqlCommand(createTempSql, connection, transaction))
                        {
                            await createCmd.ExecuteNonQueryAsync();
                        }
                        //sử dụng sqlBulkCopy để xử lý tránh các file quá lớn dẫn đến crash
                        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            //tạo index để biết dòng nào lỗi 
                            if (!table.Columns.Contains("RowIndex"))
                            {
                                table.Columns.Add("RowIndex", typeof(int));
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    table.Rows[i]["RowIndex"] = i + 2;
                                }
                            }
                            bulkCopy.DestinationTableName = tempTableName;
                            foreach (DataColumn column in table.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                            }
                            await bulkCopy.WriteToServerAsync(table);
                        }
                        //kiểm tra validate field trên bảng tạm sql bulkCopy
                        string validationSql = GenerateValidationQueryFromTempTable(tempTableName, sqlJsonDefination);
                        var validationResults = new List<IDictionary<string, object>>();
                        using (var validateCmd = new SqlCommand(validationSql, connection, transaction))
                        using (var reader = await validateCmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object?>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var value = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                    row[reader.GetName(i)] = value!;
                                }
                                if (row != null)
                                    validationResults.Add(row!);
                            }
                        }
                        var errors = validationResults
                                    .Where(r => r["ValidationResult"]?.ToString() != "Valid")
                                    .Select(r =>
                                    {
                                        var row = r.ContainsKey("RowIndex") && r["RowIndex"] != null
                                            ? Convert.ToInt32(r["RowIndex"])
                                            : -1;

                                        var error = r["ValidationResult"]?.ToString() ?? "Unknown error";

                                        return $"Row {row}, Error: {error}";
                                    })
                                    .ToList();
                        //kiểm tra database check
                        var (isValid, dbCheckErrors) = await ValidateDatabaseCheck(tempTableName, sqlJsonDefination, connection, transaction);
                        var allErrors = new List<string>();
                        if (errors.Any())
                            allErrors.AddRange(errors);
                        if (!isValid && dbCheckErrors != null)
                            allErrors.AddRange(dbCheckErrors);
                        if (allErrors.Any())
                        {
                            transaction.Rollback();
                            isRollbacked = true;
                            throw new ExceptionFormat("Invalid values: \n", allErrors);
                        }
                        //merge dữ liệu từ tempTable sang database
                        var mergeSql = GenerateMergeSqlFromTempTable(tempTableName, sqlJsonDefination);
                        using (var mergeCmd = new SqlCommand(mergeSql, connection, transaction))
                        {
                            await mergeCmd.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                        return table.Rows.Count;
                    }
                    catch (Exception ex)
                    {
                        if (!isRollbacked)
                            transaction.Rollback();

                        throw ex is ExceptionFormat ? ex : new ExceptionFormat("Error when upsert: ", new List<string> { ex.Message });
                    }
                }
            }
        }

        #endregion

        #region external function
        /// <summary>
        /// Hàm kiểm tra col table có map với col trong db hay không 
        /// </summary>
        /// <param name="oldDataTable"></param>
        /// <param name="excelColumn"></param>
        /// <returns>New Table</returns>
        public DataTable CheckExcelColumnMapping(DataTable oldDataTable, ModelDefination.ExcelIntegrationMap excelColumn)
        {
            var newDataTable = new DataTable();
            var missingColumns = new List<string>();
            // Lưu danh sách các tên cột đã map để copy dữ liệu sau
            var matchedColumns = new List<string>();
            foreach (var excelCol in excelColumn.ColumnMapping)
            {
                // Kiểm tra xem cột excel này có trong oldDataTable không
                bool columnExists = oldDataTable.Columns
                    .Cast<DataColumn>()
                    .Any(c => string.Equals(c.ColumnName.Trim(), excelCol.FieldName.Trim(), StringComparison.OrdinalIgnoreCase));

                if (excelCol.Required && !columnExists)
                {
                    // Nếu cột required mà không tồn tại thì thêm vào danh sách thiếu
                    missingColumns.Add($"Column {excelCol.FieldName} is required.");
                }
                else if (columnExists)
                {
                    // Nếu tồn tại thì thêm cột vào newDataTable
                    newDataTable.Columns.Add(excelCol.FieldName); // Dùng tên chuẩn từ mapping
                    matchedColumns.Add(excelCol.FieldName);
                }
            }
            //nếu có lỗi thì trả về lỗi luôn
            if (missingColumns.Count > 0)
            {
                throw new ExceptionFormat("Invalid Excel Format", missingColumns);
            }
            // Tạo map field -> required
            var requiredFields = new Dictionary<string, bool>();
            foreach (var col in excelColumn.ColumnMapping)
            {
                requiredFields[col.FieldName.Trim()] = col.Required;
            }
            // Thêm dữ liệu từ oldDataTable vào newDataTable chỉ với các cột đã match
            for (int rowIndex = 0; rowIndex < oldDataTable.Rows.Count; rowIndex++)
            {
                var oldRow = oldDataTable.Rows[rowIndex];
                var newRow = newDataTable.NewRow();

                foreach (string col in matchedColumns)
                {
                    var oldCol = oldDataTable.Columns
                        .Cast<DataColumn>()
                        .FirstOrDefault(c => string.Equals(c.ColumnName.Trim(), col, StringComparison.OrdinalIgnoreCase));
                    if (oldCol != null)
                    {
                        var value = oldRow[oldCol];
                        newRow[col] = value;
                        // Kiểm tra nếu là cột required và giá trị null hoặc rỗng
                        if (requiredFields[col]
                            && (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString())))
                        {
                            missingColumns.Add($"Missing value in column '{col}' at row {rowIndex + 2}"); // +2 vì 1 là header, 1 là index base 0
                        }
                    }
                }
                newDataTable.Rows.Add(newRow);
            }
            if (missingColumns.Count > 0)
            {
                throw new ExceptionFormat("Invalid Excel Format", missingColumns);
            }
            return newDataTable;
        }
        #endregion

        #region Get Template Json
        /// <summary>
        /// Hàm đọc path json dựa theo table name 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<ModelDefination> GetTemplateModelAsync(string tableName)
        {
            var folderPath = Path.Combine(AppContext.BaseDirectory, "Entities");
            if (!Directory.Exists(folderPath))
                throw new ExceptionFormat($"Folder not found: {folderPath}");
            var expectedFileName = $"{tableName}Model.json";
            // Tìm file bất kể hoa thường
            var matchedFile = Directory.GetFiles(folderPath)
                .FirstOrDefault(f => string.Equals(Path.GetFileName(f), expectedFileName, StringComparison.OrdinalIgnoreCase));
            if (matchedFile == null)
                throw new ExceptionFormat($"JSON file not match: {expectedFileName}.");
            var jsonContent = await File.ReadAllTextAsync(matchedFile);
            var template = JsonConvert.DeserializeObject<ModelDefination>(jsonContent);
            return template!;
        }

        /// <summary>
        /// Hàm đọc path json metadata dựa theo controller  
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<FormDefination> GetTemplateMetadataAsync(string controller)
        {
            try
            {
                var possiblePaths = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), "Forms"),
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Forms"),
                    Path.Combine(AppContext.BaseDirectory, "Forms") // fallback cho trường hợp publish
                };
                string? folderPath = possiblePaths.FirstOrDefault(Directory.Exists);
                if (folderPath == null)
                    throw new ExceptionFormat($"Folder not found: {folderPath}");
                var expectedFileName = $"{controller}.page.json";
                var matchedFile = Directory.GetFiles(folderPath)
                    .FirstOrDefault(f => string.Equals(Path.GetFileName(f), expectedFileName, StringComparison.OrdinalIgnoreCase));
                if (matchedFile == null)
                    throw new ExceptionFormat($"JSON file not match: {expectedFileName}.");
                var jsonContent = await File.ReadAllTextAsync(matchedFile);
                var template = JsonConvert.DeserializeObject<FormDefination>(jsonContent,
                                                                     new JsonSerializerSettings
                                                                     {
                                                                         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                                                                         DateParseHandling = DateParseHandling.None
                                                                     });
                return template!;
            }
            catch (Exception ex)
            {
                throw new ExceptionFormat($"{ex.Message}");
            }
        }

        /// <summary>
        /// Hàm lấy toàn bộ định nghĩa JSON schema cho tất cả các bảng.
        /// </summary>
        /// <returns>Danh sách SqlJsonDefination</returns>
        /// <exception cref="DirectoryNotFoundException">Nếu không tìm thấy thư mục schema</exception>
        public async Task<List<ModelDefination>> GetAllTemplateModelAsync()
        {
            var results = new List<ModelDefination>();
            string folderPath = Path.Combine(AppContext.BaseDirectory, "Entities");

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Not found folder: {folderPath}");

            var files = Directory.GetFiles(folderPath, "*Model.json");

            foreach (var file in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var def = JsonConvert.DeserializeObject<ModelDefination>(json);
                    if (def != null)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file); // ví dụ: DonHangJson
                        def.Model = fileName.Replace("Model", "");          // Lấy lại tên bảng gốc
                        results.Add(def);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error when read file {file}: {ex.Message}");
                }
            }
            return results;
        }

        /// <summary>
        /// Get primary key of table
        /// </summary>
        /// <param name="schema"></param>
        /// <returns>List or one primary key</returns>
        private List<string> GetPrimaryKeys(ModelDefination schema)
        {
            return schema.Schema.Fields
                .Where(f => f.PrimaryKey == true)
                .Select(f => f.Name)
                .ToList();
        }
        #endregion

        #region Generate sql query
        /// <summary>
        /// Hàm tạo sql để tạo table template 
        /// </summary>
        /// <param name="tempTableName"></param>
        /// <param name="sqlJsonDefination"></param>
        /// <returns></returns>
        private string GenerateCreateTableScript(string tempTableName, ModelDefination sqlJsonDefination)
        {
            if (sqlJsonDefination?.Schema?.Fields == null || !sqlJsonDefination.Schema.Fields.Any())
                throw new ArgumentException("Schema fields must be provided");

            var columns = sqlJsonDefination.Schema.Fields.Select(field =>
            $"[{field.Name}] {field.SqlType}").ToList();
            columns.Add("[RowIndex] INT");
            string columnsDefinition = string.Join(",\n", columns);
            return $"CREATE TABLE {tempTableName} (\n{columnsDefinition}\n);";
        }

        /// <summary>
        /// Hàm tạo câu lệnh merge từ bảng phụ sang bảng chính 
        /// </summary>
        /// <param name="tempTableName"></param>
        /// <param name="sqlJsonDefination"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GenerateMergeSqlFromTempTable(string tempTableName, ModelDefination sqlJsonDefination)
        {
            // Lấy tên bảng chính
            string mainTable = sqlJsonDefination.ExcelIntegration?.SheetName ?? sqlJsonDefination.Model;
            var fields = sqlJsonDefination.Schema.Fields;

            // Phân loại key / non-key
            var keyFields = fields.Where(f => f.PrimaryKey == true).ToList();
            var nonKeyFields = fields.Where(f => f.PrimaryKey != true).ToList();

            if (!keyFields.Any())
                throw new ArgumentException("Primary key field not defined in schema");

            // Alias
            string sourceAlias = "Source";
            string targetAlias = "Target";

            // Điều kiện ON theo khóa chính
            string onConditions = string.Join(" AND ",
                keyFields.Select(f => $"{targetAlias}.[{f.Name}] = {sourceAlias}.[{f.Name}]"));

            // Phần SET khi update
            string updateSet = string.Join(", ",
                nonKeyFields.Select(f => $"{targetAlias}.[{f.Name}] = {sourceAlias}.[{f.Name}]"));

            // INSERT
            string insertColumns = string.Join(", ", fields.Select(f => $"[{f.Name}]"));
            string insertValues = string.Join(", ", fields.Select(f => $"{sourceAlias}.[{f.Name}]"));

            // Final MERGE SQL
            return $@"
			MERGE INTO {mainTable} AS {targetAlias}
			USING {tempTableName} AS {sourceAlias}
			ON {onConditions}
			WHEN MATCHED THEN
				UPDATE SET {updateSet}
			WHEN NOT MATCHED THEN
				INSERT ({insertColumns})
			VALUES ({insertValues});
			";
        }

        /// <summary>
        /// Hàm này để tạo ra câu lệnh check validate theo type 
        /// </summary>
        /// <param name="tempTableName"></param>
        /// <param name="sqlDefination"></param>
        /// <returns></returns>
        private string GenerateValidationQueryFromTempTable(string tempTableName, ModelDefination sqlDefination)
        {
            var caseWhenClauses = new List<string>();

            foreach (var rule in sqlDefination.Checking.Rules)
            {
                string field = rule.FieldName;
                string message = rule.Message;
                string condition = "";

                switch (rule.Type)
                {
                    case "range":
                        condition = $"[{field}] < {rule.Min} OR [{field}] > {rule.Max}";
                        break;

                    case "length":
                        if (rule.MinLength != null)
                            caseWhenClauses.Add($"WHEN LEN([{field}]) < {rule.MinLength} THEN '{message}'");
                        if (rule.MaxLength != null)
                            caseWhenClauses.Add($"WHEN LEN([{field}]) > {rule.MaxLength} THEN '{message}'");
                        break;

                    case "pattern":
                        condition = $"[{field}] NOT LIKE '{rule.Pattern}'";
                        break;
                }

                if (!string.IsNullOrWhiteSpace(condition))
                {
                    caseWhenClauses.Add($"WHEN {condition} THEN '{message}'");
                }
            }

            string caseStatement = string.Join("\n", caseWhenClauses);

            return $@"
					SELECT *,
					CASE
						{caseStatement}
					ELSE 'Valid'
					END AS ValidationResult
					FROM {tempTableName}
					";
        }

        /// <summary>
        /// Hàm tạo câu lệnh query sử dụng open json 
        /// </summary>
        /// <param name="sqlDefination"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string GenerateValidationQueryByOpenJson(ModelDefination sqlDefination)
        {
            var caseWhenClauses = new List<string>();
            var fields = new List<string>();
            foreach (var rule in sqlDefination.Checking.Rules)
            {
                string field = rule.FieldName;
                string message = rule.Message;
                string condition = "";
                switch (rule.Type)
                {
                    case "range":
                        condition = $"[{field}] < {rule.Min} OR [{field}] > {rule.Max}";
                        break;
                    case "length":
                        if (rule.MinLength != null) caseWhenClauses.Add($"WHEN LEN([{field}]) < {rule.MinLength} THEN '{message}'");
                        if (rule.MaxLength != null) caseWhenClauses.Add($"WHEN LEN([{field}]) > {rule.MaxLength} THEN '{message}'");
                        break;
                    case "pattern":
                        condition = $"[{field}] NOT LIKE '{rule.Pattern}'";
                        break;
                }
                if (!string.IsNullOrWhiteSpace(condition))
                {
                    caseWhenClauses.Add($"WHEN {condition} THEN '{message}'");
                }
            }
            foreach (var field in sqlDefination.Schema.Fields)
            {
                var sqlDef = sqlDefination.Schema.Fields.FirstOrDefault(t => t.Name.Trim().ToLower().Equals(field.Name.Trim().ToLower()));
                if (sqlDef == null)
                {
                    throw new Exception($"Field '{field}' not exist in schema.");
                }
                string typeField = sqlDef.SqlType ?? "";
                //Chuyển kiểu không được hỗ trợ sang kiểu hợp lệ
                typeField = typeField switch
                {
                    "TEXT" => "NVARCHAR(MAX)",
                    "NTEXT" => "NVARCHAR(MAX)",
                    "IMAGE" => "VARBINARY(MAX)",
                    "SQL_VARIANT" => throw new Exception($"SQL_VARIANT is not supported in OPENJSON WITH clause. Field: {field.Name}"),
                    _ => typeField
                };
                fields.Add($"{field.Name} {typeField}");
            }
            string cases = string.Join("\n", caseWhenClauses);
            string casesField = string.Join(",\n", fields);

            return $@"
				SELECT *,
				CASE
					{cases}
					ELSE 'Valid'
				END AS ValidationResult
				FROM OPENJSON(@json)
				WITH (
					{casesField}
				)
			";
        }
        #endregion

        #region Validate
        /// <summary>
        /// Hàm xử lý validate chung open json 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        private async Task ValidateEntityAsync(object entity, ModelDefination def)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (def == null) throw new ArgumentNullException(nameof(def));

            var json = JsonConvert.SerializeObject(entity);
            var parameters = new DynamicParameters();
            parameters.Add("@json", json);

            string sqlValidation = GenerateValidationQueryByOpenJson(def);
            var validationResults = await _dbConnect.QueryAsync<dynamic>(sqlValidation, parameters);

            var errors = validationResults
                .Where(r => r.ValidationResult != "Valid")
                .Select(r => $"Row: {r}, Error: {r.ValidationResult}")
                .ToList();

            if (errors.Count > 0)
                throw new Exception("Validation error(s):\n" + string.Join("\n", errors));
        }

        /// <summary>
        /// Check duplicate
        /// </summary>
        /// <param name="oldTable"></param>
        /// <param name="sqlDefination"></param>
        /// <returns></returns>
        private async Task<(bool isValid, List<string>? errors)> ValidateDatabaseCheck(
                                                                                    string tempTableName,
                                                                                    ModelDefination sqlDefination,
                                                                                    SqlConnection connection,
                                                                                    SqlTransaction transaction)
        {
            var errors = new List<string>();
            int rowIndex = 1;
            var dbChecks = sqlDefination.Checking.Rules
                .Where(f => f.Type?.ToLower() == "databasecheck")
                .ToList();

            if (!dbChecks.Any())
                return (true, errors);

            foreach (var field in dbChecks)
            {
                string fieldName = field.FieldName!;
                string message = field.Message;
                int threshold = int.Parse(field.Threshold ?? "0");
                string tableName = sqlDefination.Model;

                //param field name
                string whereClause = field.CheckQuery!.Substring(5).Trim();
                string paramName = $"@{char.ToLowerInvariant(fieldName[0])}{fieldName.Substring(1)}";
                string onCondition = whereClause.Replace(paramName, $"t.[{fieldName}]");

                //tạo câu truy vấn kiểm tra bulk với db
                string bulkCheckQuery =
                    $@"
					SELECT t.[{fieldName}], COUNT(*) AS Total
					FROM {tableName ?? "TargetTable"} m
					JOIN {tempTableName} t ON t.[{fieldName}] = m.[{fieldName}]
					WHERE m.{onCondition}
					GROUP BY t.[{fieldName}]
					HAVING COUNT(*) > {threshold}
					";

                using var checkCmd = new SqlCommand(bulkCheckQuery, connection, transaction);
                using var reader = await checkCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var value = reader.GetValue(0);
                    var count = reader.GetInt32(1);
                    rowIndex++;
                    errors.Add($"[Row {rowIndex}]: {message} at value '{value}' (Count = {count})");
                }
                await reader.CloseAsync();

                //check duplicate trong bảng temp
                string duplicateInTempQuery =
                    $@"
					SELECT [{fieldName}], COUNT(*) AS Total
					FROM {tempTableName}
					GROUP BY [{fieldName}]
					HAVING COUNT(*) > 1";

                using (var tempCheckCmd = new SqlCommand(duplicateInTempQuery, connection, transaction))
                using (var tempReader = await tempCheckCmd.ExecuteReaderAsync())
                {
                    while (await tempReader.ReadAsync())
                    {
                        var value = tempReader.GetValue(0);
                        var count = tempReader.GetInt32(1);
                        rowIndex++;
                        errors.Add($"[Row {rowIndex}]: {fieldName} duplication at value '{value}' (Duplicated {count})");
                    }
                    await tempReader.CloseAsync();
                }
            }
            return (errors.Count == 0, errors);
        }
        #endregion
    }
}
