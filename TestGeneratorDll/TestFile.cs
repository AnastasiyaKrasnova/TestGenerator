
namespace TestGeneratorDll
{
    public class TestFile
    {
        public string FileName { get; set; }
        public string Code { get;  set; }

        public TestFile(string filename,string nmspace, string code)
        {
            FileName = nmspace+"."+filename+".cs";
            Code = code;
        }
    }
}