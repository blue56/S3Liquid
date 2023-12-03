namespace S3Liquid;

public class S3LiquidRequest
{
    public string Region { get; set; }
    public string Bucketname { get; set; }
    public string TemplatePath { get; set; }
    public string OutputPath {get; set;}
    public Input[] Inputs {get; set;}
    public bool Overwrite{get; set;}
}