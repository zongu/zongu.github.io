<Query Kind="Program" />

void Main()
{
//var data = A_Members.Select(x => x.MemberID).Take(10);
//Console.WriteLine(data);	
this.Connection
.DumpClass(@"
		EXEC Game_Agent.dbo.NSP_AgentWeb_PincodeCreateLogList
			@stardate		 = '2016-07-01 18:38:06.090'
			,@enddate		 = '2016-08-30 13:57:17.293'
			,@PincodeEventID = ''
			,@Environment	 = 'QC3'
		").Dump();

}

// Define other methods and classes here

public static class LINQPadExtensions
{
    private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string> {
        { typeof(int), "int" },
        { typeof(short), "short" },
        { typeof(byte), "byte" },
        { typeof(byte[]), "byte[]" },
        { typeof(long), "long" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(float), "float" },
        { typeof(bool), "bool" },
        { typeof(string), "string" }
    };
     
    private static readonly HashSet<Type> NullableTypes = new HashSet<Type> {
        typeof(int),
        typeof(short),
        typeof(long),
        typeof(double),
        typeof(decimal),
        typeof(float),
        typeof(bool),
        typeof(DateTime)
    };
 
    public static string DumpClass(this IDbConnection connection, string sql, string className = "Info")
    {
        if(connection.State != ConnectionState.Open)
        {
   connection.Open();
  }
             
        var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        var reader = cmd.ExecuteReader();
                         
        var builder = new StringBuilder();
        do
        {
            if(reader.FieldCount <= 1) continue;
         
            builder.AppendFormat("public class {0}{1}", className, Environment.NewLine);
            builder.AppendLine("{");
            var schema = reader.GetSchemaTable();
                         
            foreach (DataRow row in schema.Rows)
            {
                var type = (Type)row["DataType"];
                var name = TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.Name;
                var isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(type);
                var collumnName = (string)row["ColumnName"];
                 
                builder.AppendLine(string.Format("\tpublic {0}{1} {2} {{ get; set; }}", name, isNullable ? "?" : string.Empty, collumnName));
    builder.AppendLine();
            }
             
            builder.AppendLine("}");
            builder.AppendLine();            
        } while(reader.NextResult());
         
        return builder.ToString();
    }
}