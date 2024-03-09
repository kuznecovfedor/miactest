﻿namespace MIACApi
{
    public static class DBExceptionMatcher
    {
        public static (int status, string? message) GetByExceptionMessage(string message)
        {
            return message switch
            {
                string a when a.Contains("Duplicate entry") => (400, "Добавление дубликата"),
                string a when a.Contains("foreign key constraint") => (400, "Ошибка связанной таблицы"),
                string a when a.Contains("check constraint") => (400, "Некорректные данные"),
                _ => (500, null)
            };
        }
    }
}