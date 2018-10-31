namespace CodeGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GetCode getCode = new GetCode();

            CodeAppUsers(getCode);

            getCode.Run("output.txt");
        }

        private static void CodeAppUsers(GetCode getCode)
        {
            CodeModel myCodeModel = getCode.MyCodeModel;

            myCodeModel.ModuleName = "AppUser";
            myCodeModel.ContextName = "AppUsers";
            myCodeModel.ClassName = "AppUser";

            myCodeModel.Fields.Add(new FieldModel()
            {
                FieldName = "Id",
                IsString = true
            });

            myCodeModel.Fields.Add(new FieldModel()
            {
                FieldName = "Email",
                IsString = true,
                IsInput = true,
            });

            myCodeModel.Fields.Add(new FieldModel()
            {
                FieldName = "EmailConfirmed",
                IsBool = true,
                IsInput = true,
            });

            myCodeModel.Fields.Add(new FieldModel()
            {
                FieldName = "PhoneNumber",
                IsString = true,
                IsInput = true,
            });

            myCodeModel.Fields.Add(new FieldModel()
            {
                FieldName = "UserName",
                IsString = true,
                IsInput = true,
            });
        }

    }

}
