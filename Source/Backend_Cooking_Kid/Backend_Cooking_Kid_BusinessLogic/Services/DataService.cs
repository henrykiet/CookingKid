using Backend_Cooking_Kid_BusinessLogic.DTOs;
using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.Helps;
using Backend_Cooking_Kid_DataAccess;
using Backend_Cooking_Kid_DataAccess.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Cooking_Kid_BusinessLogic.Services
{
    public interface IDataService
    {
        Task<ServiceResponse<object>> GetFormMetadataAsync(MetadataRequest? request);
        Task<ServiceResponse<object>> UpdateFormMetadataAsync();
    }
    public class DataService : IDataService
    {
        private readonly IBaseRepository<ExpandoObject> _repository;
        public DataService(IBaseRepository<ExpandoObject> baseRepository)
        {
            _repository = baseRepository;
        }

        public async Task<ServiceResponse<object>> GetFormMetadataAsync(MetadataRequest? request)
        {
            var result = new ServiceResponse<object>();
            string controller = "";
            if (request != null && request.Controller != null)
            {
                controller = request.Controller;
                var formResult = await _repository.GetTemplateMetadataAsync(controller);
                if (formResult == null)
                {
                    result.Success = false;
                    result.Message = "Không tìm thấy dữ liệu";
                    result.StatusCode = 404;
                    return result;
                }
                if (formResult.Form != null && !string.IsNullOrEmpty(request.Action))
                {
                    var jArray = new JArray();
                    var isMenus = formResult.Controller.Equals("menus", StringComparison.OrdinalIgnoreCase);
                    var action = request.Action.ToLower().Trim();
                    switch (action)
                    {
                        case "insert":
                            result.Success = true;
                            result.Data = formResult;
                            result.Message = "Lấy dữ liệu thành công";
                            result.StatusCode = 200;
                            return result;
                        case "list":
                            //chỉ trả về list master ngoại trừ menus
                            // Master form
                            jArray = await GetInitialDataAsync(formResult.Form.TableName, formResult.Form.FieldControls, null);

                            formResult.Form!.InitialDatas = jArray != null
                                ? jArray.Select(j => j.ToObject<Dictionary<string, string>>()!).ToList()
                                : new List<Dictionary<string, string>>();
                            if (isMenus)
                            {
                                // Detail forms 
                                if (formResult.Form.DetailForms != null)
                                {
                                    foreach (var detailForm in formResult.Form.DetailForms)
                                    {
                                        var jArrayDetail = await GetInitialDataAsync(detailForm.TableName, detailForm.FieldControls, request.PkValue);

                                        detailForm.InitialDatas = jArrayDetail != null
                                            ? jArrayDetail.Select(j => j.ToObject<Dictionary<string, string>>()!).ToList()
                                            : new List<Dictionary<string, string>>();
                                    }
                                }
                            }
                            break;
                        case "update":
                            // Master form
                            //check có trung với form result không
                            if (request.PkValue != null)
                                foreach (var pkVal in request.PkValue)
                                {
                                    var pk = formResult.Form.PrimaryKey != null && formResult.Form.PrimaryKey.Contains(pkVal.Key);
                                    if (!pk)
                                    {
                                        throw new Exception($"Khóa chính ({pkVal.Key}) không hợp lệ");
                                    }
                                }
                            //Hợp lệ khóa  master
                            jArray = await GetInitialDataAsync(formResult.Form.TableName, formResult.Form.FieldControls, request.PkValue);

                            formResult.Form!.InitialDatas = jArray != null
                                ? jArray.Select(j => j.ToObject<Dictionary<string, string>>()!).ToList()
                                : new List<Dictionary<string, string>>();

                            // Lấy detail forms dựa trên khóa chính master
                            if (formResult.Form.DetailForms != null && request.Action != null
                                && (request.Action.Equals("update", StringComparison.OrdinalIgnoreCase)))
                            {
                                foreach (var detailForm in formResult.Form.DetailForms)
                                {
                                    var jArrayDetail = await GetInitialDataAsync(detailForm.TableName, detailForm.FieldControls, request.PkValue);

                                    detailForm.InitialDatas = jArrayDetail != null
                                        ? jArrayDetail.Select(j => j.ToObject<Dictionary<string, string>>()!).ToList()
                                        : new List<Dictionary<string, string>>();
                                }
                            }
                            break;
                        default:
                            result.Success = false;
                            result.Data = null;
                            result.Message = "Action không hợp lệ";
                            result.StatusCode = 500;
                            return result;
                    }
                    result.Success = true;
                    result.Data = formResult;
                    result.Message = "Lấy dữ liệu thành công";
                    result.StatusCode = 200;
                }
            }
            return result;
        }

        // Hàm chung lấy InitialData từ table + fieldControls
        private async Task<JArray?> GetInitialDataAsync(string? tableName, List<FieldControl>? fieldControls, Dictionary<string, string>? pkValue)
        {
            var initialDatas = new JArray();

            if (string.IsNullOrEmpty(tableName) || fieldControls == null)
                return initialDatas;
            var items = new List<dynamic>();
            if (pkValue == null)
            {
                items = await _repository.GetAllAsync(tableName, null, null);
            }
            else if (pkValue != null)
            {

                items = await _repository.GetAllAsync(tableName, pkValue, null, null);
            }
            else
            {
                return initialDatas;
            }

            foreach (var row in items)
            {
                var rowObject = new JObject();
                if (row is IDictionary<string, object> dict)
                {
                    foreach (var field in fieldControls)
                    {
                        var fieldName = field.Name;
                        if (string.IsNullOrEmpty(fieldName)) continue;

                        dict.TryGetValue(fieldName, out var value);
                        rowObject[fieldName] = value != null ? JToken.FromObject(value) : JValue.CreateNull();
                    }
                }
                initialDatas.Add(rowObject);
            }

            return initialDatas;
        }


        public async Task<ServiceResponse<object>> UpdateFormMetadataAsync()
        {
            var result = new ServiceResponse<object>();
            var controller = "update";
            var formResult = await _repository.GetTemplateMetadataAsync(controller);
            if (formResult == null)
            {
                result.Success = false;
                result.Message = "Không tìm thấy dữ liệu";
                result.StatusCode = 404;
            }
            else
            {
                result.Data = formResult;
                result.Message = "Lấy dữ liệu thành công";
                result.StatusCode = 200;
            }
            return result;
        }
    }
}
