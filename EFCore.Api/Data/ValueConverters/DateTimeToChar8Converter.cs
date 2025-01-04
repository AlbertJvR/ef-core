using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EFCore.Api.Data.ValueConverters;

public class DateTimeToChar8Converter : ValueConverter<DateTime, string>
{
    public DateTimeToChar8Converter() : base(
        datetime => datetime.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
        stringValue => DateTime.ParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture))
    {
    }
}