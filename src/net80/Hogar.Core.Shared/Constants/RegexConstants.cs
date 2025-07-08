namespace Hogar.Core.Shared.Constants;

public static class RegexConstants
{
    public const string RGX_LOCATION_PATTERN = @"^([-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)),\s*([-+]?(180(\.0+)?|(1[0-7]\d|\d{1,2})(\.\d+)?))$";
    public const string RGX_CURP_PATTERN = @"^[A-Z]{1}[AEIOUX]{1}[A-Z]{2}\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])[HM]{1}(AS|BC|BS|CC|CL|CM|CS|CH|DF|DG|GT|GR|HG|JC|MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|TL|VZ|YN|ZS)[B-DF-HJ-NP-TV-Z]{3}[A-Z\d]{1}\d{1}$";
    public const string RGX_RFC_PATTERN = @"^[A-ZÑ&]{3,4}\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])[A-Z\d]{3}$";
    public const string RGX_GENRE_PATTERN = @"^[MF]$";
    public const string RGX_CIC_MINUS_THAN_2013 = @"^\d{9}$";
    public const string RGX_CIC_MORE_THAN_2013 = @"^\d{9,10}$";
    public const string RGX_OCR_MORE_THAN_2013 = @"^\d{12,13}$";
    public const string RGX_CODE_CONSTITUENT = @"^[A-Z0-9]{18}$";
    public const string RGX_CODE_EMISSION = @"^\d{2}$";
    public const string RGX_UUID_V4_PATTERN = @"^[{(]?[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){1}-4[0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}(-[0-9a-fA-F]{12})[)}]?$";
    public const string RGX_DATE_DASH_YMD_HM_PATTERN = @"[0-9]{4}-(((0[13578]|(10|12))-(0[1-9]|[1-2][0-9]|3[0-1]))|(02-(0[1-9]|[1-2][0-9]))|((0[469]|11)-(0[1-9]|[1-2][0-9]|30)))\s[0-9]{2}[:]{1}[0-9]{2}$";
    public const string RGX_SPACE_CLEAR = @"\s+";
    public const string RGX_MULTIPLE_SPACE_CLEAR = @"^\s+$";
    public const string RGX_JWT_TOKEN = @"^Bearer\s[a-zA-Z0-9\-\._~\+\/]+=*$";
}
