using Azure.Core;
using Backend_Cooking_Kid_BusinessLogic.DTOs;
using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.Helps;
using Backend_Cooking_Kid_DataAccess;
using Backend_Cooking_Kid_DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Cooking_Kid_BusinessLogic.Services
{
    public interface IDataService
    {
        Task<ServiceResponse<object>> GetFormMetadataAsync(MetadataRequest? request);
        Task<ServiceResponse<object>> UpdateFormMetadataAsync(UpdateMetadataRequest request);
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
                    result.Message = $"Not found {controller} json file";
                    result.StatusCode = 404;
                    return result;
                }

                if (formResult.Form != null && !string.IsNullOrEmpty(request.Action))
                {
                    var jArray = new JArray();
                    var isMenus = formResult.Controller.Equals("menus", StringComparison.OrdinalIgnoreCase);
                    var action = request.Action.ToLower().Trim();
                    var partition = DateTime.Now.ToString("yyyyMM");
                    var tableName = formResult.IsPartition == true ? formResult.Form.TableName + $"${partition}" : formResult.Form.TableName;
                    var fieldControls = formResult.Form.FieldControls;

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
                            jArray = await GetInitialDataAsync(tableName, fieldControls, null);

                            formResult.Form!.InitialDatas = jArray != null
                                ? jArray.Select(j => j.ToObject<Dictionary<string, object>>()!).ToList()
                                : new List<Dictionary<string, object>>();
                            if (isMenus)
                            {
                                // Detail forms 
                                if (formResult.Form.DetailForms != null)
                                {
                                    foreach (var detailForm in formResult.Form.DetailForms)
                                    {
                                        var jArrayDetail = await GetInitialDataAsync(detailForm.TableName, detailForm.FieldControls, request.PkValue);

                                        detailForm.InitialDatas = jArrayDetail != null
                                            ? jArrayDetail.Select(j => j.ToObject<Dictionary<string, object>>()!).ToList()
                                            : new List<Dictionary<string, object>>();
                                    }
                                }
                            }
                            break;
                        case "update":
                            if (request.PkValue == null)
                            {
                                result.Success = false;
                                result.Data = null;
                                result.Message = "PK value is null or empty";
                                return result;
                            }
                            // Master form
                            jArray = await GetInitialDataAsync(tableName, fieldControls, request.PkValue);

                            formResult.Form!.InitialDatas = jArray != null
                                ? jArray.Select(j => j.ToObject<Dictionary<string, object>>()!).ToList()
                                : new List<Dictionary<string, object>>();

                            // detail forms dựa trên khóa chính master
                            if (formResult.Form.DetailForms != null && request.Action != null
                                && (request.Action.Equals("update", StringComparison.OrdinalIgnoreCase)))
                            {
                                foreach (var detailForm in formResult.Form.DetailForms)
                                {
                                    var detailTableName = formResult.IsPartition == true
                                                                    ? detailForm.TableName + $"${partition}"
                                                                    : detailForm.TableName;
                                    var jArrayDetail = await GetInitialDataAsync(detailTableName, detailForm.FieldControls, request.PkValue);

                                    detailForm.InitialDatas = jArrayDetail != null
                                        ? jArrayDetail.Select(j => j.ToObject<Dictionary<string, object>>()!).ToList()
                                        : new List<Dictionary<string, object>>();
                                    detailForm.TableName = detailTableName;
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
                    formResult.Form.TableName = tableName;
                    formResult.Partition = partition;
                    result.Success = true;
                    result.Data = formResult;
                    result.Message = "Lấy dữ liệu thành công";
                    result.StatusCode = 200;
                }
            }
            return result;
        }

        public async Task<ServiceResponse<object>> UpdateFormMetadataAsync(UpdateMetadataRequest request)
        {
            var result = new ServiceResponse<object>();
            if (string.IsNullOrEmpty(request.Controller))
            {
                result.Success = false;
                result.Message = $"Not have controller send by FE";
                result.StatusCode = 404;
                return result;
            }
            var controller = request.Controller;
            var formResult = await _repository.GetTemplateMetadataAsync(controller);
            if (formResult == null)
            {
                result.Success = false;
                result.Message = $"Not found {controller} json file";
                result.StatusCode = 404;
                return result;
            }
            try
            {
                var initialDatas = JsonHelper.ConvertJsonInitialToDictionary(request.InitialDatas);

                if (initialDatas == null)
                {
                    result.Success = false;
                    result.Message = "Convert master from json to object fail or data is invalid.";
                    return result;
                }
                var masterForm = formResult.Form!;
                var masterFormValues = new Dictionary<string, object>(initialDatas);

                //xử lý detail nếu có trước
                if (masterForm.DetailForms != null && masterForm.DetailForms.Any())
                {
                    foreach (var detailForm in masterForm.DetailForms)
                    {
                        if (string.IsNullOrEmpty(detailForm.TableName)) throw new ArgumentException(nameof(detailForm.TableName));
                        var detailTableName = formResult.IsPartition == true ? detailForm.TableName + "$" + (request.Partition ?? "") : detailForm.TableName;
                        //gán lại tableName 
                        detailForm.TableName = detailTableName;
                        if (masterFormValues.ContainsKey(detailTableName))
                        {
                            var convertInitialDetail = JsonHelper.ConvertJsonArrayToDictionaries(masterFormValues[detailTableName]);
                            if (convertInitialDetail == null)
                            {
                                result.Success = false;
                                result.Message = "Convert initial detail array null or empty";
                                result.StatusCode = 500;
                                return result;
                            }

                            var detailUpdateResult = await UpdateInitialToDBAsync(detailForm, convertInitialDetail);

                            if (detailUpdateResult.Item1 == false)
                            {
                                result.Success = false;
                                result.Message = detailUpdateResult.Item2;
                                result.StatusCode = 500;
                                return result;
                            }

                            // Xóa Key của Detail khỏi dữ liệu Master
                            masterFormValues.Remove(detailTableName);
                        }
                    }
                }

                //Xử lý master
                var masterTableName = formResult.IsPartition == true ? masterForm.TableName + "$" + (request.Partition ?? "") : masterForm.TableName;
                masterForm.TableName = masterTableName;
                //convert json master
                var convertInitialMaster = JsonHelper.ConverJsonDataAsDictionary(masterFormValues);
                if(convertInitialMaster == null)
                {
                    result.Success = false;
                    result.Message = "Convert initial master fail";
                    result.StatusCode = 500;
                    return result;
                }
                var masterUpdateResult = await UpdateInitialToDBAsync(masterForm, convertInitialMaster);
                if (masterUpdateResult.Item1 == false)
                {
                    result.Success = false;
                    result.Message = masterUpdateResult.Item2;
                    result.StatusCode = 500;
                    return result;
                }

                result.Success = true;
                result.Message = "Update Successfully";
                result.StatusCode = 200;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.StatusCode = 500;
            }
            return result;
        }


        //Hàm update từng table
        private async Task<(bool, string)> UpdateInitialToDBAsync(IForm form, object initialDatas)
        {
            try
            {
                if (string.IsNullOrEmpty(form.TableName)) throw new ArgumentException(nameof(form.TableName));

                IEnumerable<Dictionary<string, object>> rowsToProcess;
                if (initialDatas is IEnumerable<Dictionary<string, object>> detailRows)
                {
                    rowsToProcess = detailRows;
                }
                // Nếu là Master Form (Một hàng): Dữ liệu là Dictionary đơn lẻ
                else if (initialDatas is Dictionary<string, object> masterRow)
                {
                    rowsToProcess = new List<Dictionary<string, object>> { masterRow };
                }
                else
                {
                    return (false, "Initial data is not a valid single record or list of records.");
                }
                var pkVals = new List<Dictionary<string, object>>();
                var datas = new List<Dictionary<string, object>>();


                if (form.FieldControls == null) throw new ArgumentException(nameof(form.FieldControls));
                if (form.PrimaryKey == null) return (false, "Not have primary key defination");

                var pkSet = new HashSet<string>(form.PrimaryKey);
                var fieldNames = new HashSet<string>(
                                form.FieldControls
                                        .Where(f => !string.IsNullOrEmpty(f.Name))
                                        .Select(f => f.Name!)
                                );
                foreach (var row in rowsToProcess)
                {
                    var pkVal = new Dictionary<string, object>();
                    var data = new Dictionary<string, object>();
                    foreach (var d in row)
                    {
                        //nếu là key
                        if (pkSet.Contains(d.Key))
                        {
                            pkVal.Add(d.Key, d.Value);
                        }
                        else
                        {
                            if (fieldNames.Contains(d.Key))
                                data.Add(d.Key, d.Value);
                        }
                    }
                    pkVals.Add(pkVal);
                    datas.Add(data);
                }

                var formResult = await _repository.UpdateAsync(form.TableName, pkVals, datas);
                if (formResult)
                {
                    return (true, "Succes update");
                }
                else
                {
                    return (false, $"Fail to update {form.TableName}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"UpdateInitialToDBAsync error: {ex.Message}");
            }
        }

        // Hàm chung lấy InitialData từ table + fieldControls
        private async Task<JArray?> GetInitialDataAsync(string? tableName, List<FieldControl>? fieldControls, Dictionary<string, object>? pkValue)
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
                items = await _repository.GetAllByIdAsync(tableName, JsonHelper.ConverJsonDataAsDictionary(pkValue), null, null);
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
    }
}
