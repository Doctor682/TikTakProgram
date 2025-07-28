using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikTakProgram.Dtos;

namespace TikTakProgram
{
    public static class ErrorHandler
    {
        public static void PrintServerError(string rawJson)
        {
            try
            {
                ErrorResponse? errorObj = JsonConvert.DeserializeObject<ErrorResponse>(rawJson);
                if (errorObj != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    string userMessage = ErrorMessages.GetMessageByCode(errorObj.code);

                    Console.WriteLine($"[Код: {errorObj.code}] {userMessage}");

                    if (!string.IsNullOrWhiteSpace(errorObj.error))
                    {
                        Console.WriteLine($"Системна помилка: {errorObj.error}");
                    }

                    if (errorObj.details != null)
                    {
                        foreach (KeyValuePair<string, string?> kv in errorObj.details)
                            Console.WriteLine($"  ▪ {kv.Key}: {kv.Value}");
                    }

                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка обробки відповіді сервера] {ex.Message}");
            }
        }
    }

}
