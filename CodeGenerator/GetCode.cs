using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class GetCode
    {
        public string TemplatePath = ".\\spa_webapi_template.txt";
        public CodeModel MyCodeModel = new CodeModel();

        public GetCode()
        {
        }

        public void Run(string output)
        {
            var builder = new StringBuilder();
            var lines = File.ReadLines(TemplatePath).ToList();
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i];

                line = ProcessLine(line);

                builder.AppendLine(line);
            }
            File.WriteAllText(output, builder.ToString());
        }

        private string ProcessLine(string line)
        {
            if (line.Contains("%ModuleName%"))
            {
                line = line.Replace("%ModuleName%", MyCodeModel.ModuleName);
            }
            if (line.Contains("%ModuleNameLowercase%"))
            {
                line = line.Replace("%ModuleNameLowercase%", GetFirstLetterLowerCase(MyCodeModel.ModuleName));
            }
            if (line.Contains("%ContextName%"))
            {
                line = line.Replace("%ContextName%", MyCodeModel.ContextName);
            }
            if (line.Contains("%ClassName%"))
            {
                line = line.Replace("%ClassName%", MyCodeModel.ClassName);
            }
            if (line.Contains("%ClassNameLowercase%"))
            {
                line = line.Replace("%ClassNameLowercase%", GetFirstLetterLowerCase(MyCodeModel.ClassName));
            }
            if (line.Contains("%ClassNameWithSpace%"))
            {
                line = line.Replace("%ClassNameWithSpace%", InsertSpace(MyCodeModel.ClassName));
            }
            if (line.Contains("%LINEFEED%"))
            {
                line = line.Replace("%LINEFEED%", "\r\n");
            }
            if (line.Contains("%Field%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    var tmpLine = templateLine.Replace("%Field%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldWithoutYearMonth%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsYearMonth)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldWithoutYearMonth%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldWithoutAuto%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsAuto)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldWithoutAuto%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldWithoutYearMonthAndAuto%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsYearMonth || field.IsAuto)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldWithoutYearMonthAndAuto%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldWithoutYearMonthAndDate%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                var isYearMonthFound = false;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate)
                    {
                        continue;
                    }
                    if (!isYearMonthFound && field.IsYearMonth)
                    {
                        isYearMonthFound = true;
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldWithoutYearMonthAndDate%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldWithoutYearMonthAndDateLowerCase%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                var isYearMonthFound = false;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate)
                    {
                        continue;
                    }
                    if (!isYearMonthFound && field.IsYearMonth)
                    {
                        isYearMonthFound = true;
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldWithoutYearMonthAndDateLowerCase%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldInput%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (!field.IsInput)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldInput%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldInputLowerCase%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (!field.IsInput)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldInputLowerCase%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldInputWithoutDate%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate || !field.IsInput)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldInputWithoutDate%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldInputWithoutDateLowerCase%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate || !field.IsInput)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldInputWithoutDateLowerCase%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldStat%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (!field.IsStat)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldStat%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldStatLowerCase%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (!field.IsStat)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldStatLowerCase%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldStatCount%"))
            {
                line = line.Replace("%FieldStatCount%", string.Format("{0}", MyCodeModel.FieldStatCount())) + "\r\n";
            }
            if (line.Contains("%FieldStatCount+1%"))
            {
                line = line.Replace("%FieldStatCount+1%", string.Format("{0}", MyCodeModel.FieldStatCount()+1)) + "\r\n";
            }
            if (line.Contains("%FieldSum%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (!field.IsSum)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldSum%", field.FieldName) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldSumLowerCase%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (!field.IsSum)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldSumLowerCase%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FieldSumIndex%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (!field.IsSum)
                    {
                        continue;
                    }
                    var tmpLine = templateLine.Replace("%FieldSumIndex%", string.Format("{0}", index)) + "\r\n";
                    line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                }
            }
            if (line.Contains("%FirstFieldDate%"))
            {
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate)
                    {
                        line = line.Replace("%FirstFieldDate%", field.FieldName);
                        break;
                    }
                }
            }
            if (line.Contains("%FieldLengthCheck2%"))
            {
                var tmp = "";
                for (int i = 0; i < MyCodeModel.Fields.Count; i++)
                {
                    tmp += string.Format("&& fields[{0}].Length == 0", i + 2);
                }
                line = line.Replace("%FieldLengthCheck2%", tmp);
            }
            if (line.Contains("%FieldString%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsString)
                    {
                        var tmpLine = templateLine.Replace("%FieldString%", field.FieldName) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldStringFirstLetterLower%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsString)
                    {
                        var tmpLine = templateLine.Replace("%FieldStringFirstLetterLower%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldNumber%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsNumber && !field.IsYearMonth)
                    {
                        var tmpLine = templateLine.Replace("%FieldNumber%", field.FieldName) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldNumberWithoutAuto%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsNumber && !field.IsYearMonth && !field.IsAuto)
                    {
                        var tmpLine = templateLine.Replace("%FieldNumberWithoutAuto%", field.FieldName) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldNumberFirstLetterLower%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsNumber && !field.IsYearMonth)
                    {
                        var tmpLine = templateLine.Replace("%FieldNumberFirstLetterLower%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldBool%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsBool)
                    {
                        var tmpLine = templateLine.Replace("%FieldBool%", field.FieldName) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldBoolFirstLetterLower%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsBool)
                    {
                        var tmpLine = templateLine.Replace("%FieldBoolFirstLetterLower%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldDate%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate)
                    {
                        var tmpLine = templateLine.Replace("%FieldDate%", field.FieldName) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FieldDateFirstLetterLower%"))
            {
                var templateLine = line;
                line = "";
                var index = 0;
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate)
                    {
                        var tmpLine = templateLine.Replace("%FieldDateFirstLetterLower%", GetFirstLetterLowerCase(field.FieldName)) + "\r\n";
                        line += ProcessFieldIndex(tmpLine, index++, field.FieldName);
                    }
                }
            }
            if (line.Contains("%FirstFieldDateFirstLetterLower%"))
            {
                foreach (var field in MyCodeModel.Fields)
                {
                    if (field.IsDate)
                    {
                        line = line.Replace("%FirstFieldDateFirstLetterLower%", GetFirstLetterLowerCase(field.FieldName));
                        break;
                    }
                }
            }
            return line;
        }

        private string ProcessFieldIndex(string line, int index, string field)
        {
            if (line.Contains("%FieldIndex%"))
            {
                line = line.Replace("%FieldIndex%", string.Format("{0}", index));
            }
            if (line.Contains("%FieldIndex2%"))
            {
                line = line.Replace("%FieldIndex2%", string.Format("{0}", index + 2));
            }
            if (line.Contains("%FieldLowerCaseFirst%"))
            {
                line = line.Replace("%FieldLowerCaseFirst%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldNumberFirstLetterLower%"))
            {
                line = line.Replace("%FieldNumberFirstLetterLower%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldBoolFirstLetterLower%"))
            {
                line = line.Replace("%FieldBoolFirstLetterLower%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldNumber%"))
            {
                line = line.Replace("%FieldNumber%", field);
            }
            if (line.Contains("%FieldBool%"))
            {
                line = line.Replace("%FieldBool%", field);
            }
            if (line.Contains("%FieldWithSpace%"))
            {
                line = line.Replace("%FieldWithSpace%", InsertSpace(field));
            }
            if (line.Contains("%FieldString%"))
            {
                line = line.Replace("%FieldString%", field);
            }
            if (line.Contains("%FieldStringFirstLetterLower%"))
            {
                line = line.Replace("%FieldStringFirstLetterLower%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldWithoutAuto%"))
            {
                line = line.Replace("%FieldWithoutAuto%", field);
            }
            if (line.Contains("%FieldWithoutYearMonthAndAuto%"))
            {
                line = line.Replace("%FieldWithoutYearMonthAndAuto%", field);
            }
            if (line.Contains("%FieldWithoutYearMonthAndDate%"))
            {
                line = line.Replace("%FieldWithoutYearMonthAndDate%", field);
            }
            if (line.Contains("%FieldWithoutYearMonthAndDateLowerCase%"))
            {
                line = line.Replace("%FieldWithoutYearMonthAndDateLowerCase%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldInput%"))
            {
                line = line.Replace("%FieldInput%", field);
            }
            if (line.Contains("%FieldInputLowerCase%"))
            {
                line = line.Replace("%FieldInputLowerCase%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldInputWithoutDate%"))
            {
                line = line.Replace("%FieldInputWithoutDate%", field);
            }
            if (line.Contains("%FieldInputWithoutDateLowerCase%"))
            {
                line = line.Replace("%FieldInputWithoutDateLowerCase%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldStat%"))
            {
                line = line.Replace("%FieldStat%", field);
            }
            if (line.Contains("%FieldStatLowerCase%"))
            {
                line = line.Replace("%FieldStatLowerCase%", GetFirstLetterLowerCase(field));
            }
            if (line.Contains("%FieldSum%"))
            {
                line = line.Replace("%FieldSum%", field);
            }
            if (line.Contains("%FieldSumLowerCase%"))
            {
                line = line.Replace("%FieldSumLowerCase%", GetFirstLetterLowerCase(field));
            }
            return line;
        }
        private static string GetFirstLetterLowerCase(string field)
        {
            return string.Format("{0}{1}", field.Substring(0, 1).ToLowerInvariant(), field.Substring(1));
        }

        private string InsertSpace(string field)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < field.Length; i++)
            {
                if (i > 0 && char.IsUpper(field[i]))
                {

                    builder.Append(' ');
                }

                builder.Append(field[i]);
            }
            return builder.ToString();
        }

    }

}
