using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Cooking_Kid_BusinessLogic.Helps
{
    public class JsonHelper
    {

        public static List<Dictionary<string, object>>? ConvertJsonArrayToDictionaries(object? jsonArrayValue)
        {
            if (jsonArrayValue == null) return null;

            string jsonString;

            // Nếu nó là JsonElement hoặc đã là string, lấy chuỗi JSON
            if (jsonArrayValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
            {
                jsonString = jsonElement.GetRawText();
            }
            else if (jsonArrayValue is string s)
            {
                jsonString = s;
            }
            else
            {
                return null; // Không phải kiểu hỗ trợ
            }

            try
            {
                // 💡 Deserialize thẳng sang List<Dictionary<string, object>>
                // Điều này chỉ hoạt động nếu mỗi item trong mảng là một đối tượng JSON
                var rawList = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString);

                // Cần thêm bước làm sạch (cleaning) cho từng Dictionary con
                if (rawList != null)
                {
                    return rawList.Select(item => ConverJsonDataAsDictionary(item)!).ToList();
                }

                return rawList;
            }
            catch (System.Text.Json.JsonException)
            {
                return null; // Trả về null nếu JSON không hợp lệ
            }
        }


        // Đặt trong JsonHelper
        public static Dictionary<string, object>? ConvertJsonInitialToDictionary(object? initialDataValue)
        {
            if (initialDataValue == null) return null;

            // Trường hợp 1: Dữ liệu là JsonElement (từ quá trình deserialization JSON)
            if (initialDataValue is JsonElement jsonElement)
            {
                try
                {
                    // Chuyển đổi JsonElement sang Dictionary<string, object>
                    string jsonString = jsonElement.GetRawText();
                    return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
                }
                catch (System.Text.Json.JsonException)
                {
                    return null; // Trả về null nếu JSON không hợp lệ
                }
            }
            // Trường hợp 2: Dữ liệu đã là Dictionary<string, object>
            else if (initialDataValue is Dictionary<string, object> dict)
            {
                return dict;
            }

            return null; // Không phải kiểu hỗ trợ
        }

        public static Dictionary<string, object>? ConverJsonDataAsDictionary(Dictionary<string, object> data)
        {
            var cleanedData = new Dictionary<string, object>();

            foreach (var kvp in data)
            {
                object value = kvp.Value;

                if (value is JsonElement jsonElement)
                {
                    // Ép kiểu dựa trên loại giá trị JSON
                    switch (jsonElement.ValueKind)
                    {
                        case JsonValueKind.String:
                            value = jsonElement.GetString()!; // Lấy giá trị chuỗi
                            break;
                        case JsonValueKind.Number:
                            // Ưu tiên số nguyên, sau đó là số thập phân
                            if (jsonElement.TryGetInt64(out long l))
                                value = l;
                            else if (jsonElement.TryGetDecimal(out decimal d))
                                value = d;
                            else
                                value = jsonElement.GetRawText(); // Dự phòng
                            break;
                        case JsonValueKind.True:
                            value = true;
                            break;
                        case JsonValueKind.False:
                            value = false;
                            break;
                        case JsonValueKind.Null:
                            value = DBNull.Value; // Dapper chấp nhận DBNull.Value cho NULL
                            break;
                        default:
                            // Đối tượng hoặc Mảng phức tạp (bỏ qua hoặc giữ nguyên dạng JsonElement nếu không biết cách xử lý)
                            value = jsonElement.GetRawText();
                            break;
                    }
                }

                cleanedData.Add(kvp.Key, value);
            }

            return cleanedData;
        }

    }
}
