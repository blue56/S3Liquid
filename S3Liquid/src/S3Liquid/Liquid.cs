using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Fluid;
using Fluid.Values;
using Newtonsoft.Json.Linq;

namespace S3Liquid;

public class Liquid
{
    public void Run(S3LiquidRequest Request)
    {
        var parser = new FluidParser();

        // Read template content from S3 file
        string templateText = GetFileContent(Request.Region,
            Request.Bucketname, Request.TemplatePath);

        if (parser.TryParse(templateText, out var template, out var error))
        {
            var templateContext = MakeTemplateContext();

            // add each input to template context
            foreach (var input in Request.Inputs)
            {
                // Read source content from S3 file
                string jsonString = GetFileContent(Request.Region,
                    Request.Bucketname, input.Path);

                JContainer sourceDocument = (JContainer)JToken.Parse(jsonString);

                templateContext.SetValue(input.Alias, sourceDocument);
            }

            string resultString = template.Render(templateContext);

            // Save to file
            var stream = new MemoryStream(
                    Encoding.UTF8.GetBytes(resultString));

            SaveFile(Request.Region, Request.Bucketname,
                Request.OutputPath, stream, "application/json");
        }
    }

    public void SaveFile(string Region, string Bucketname,
        string Key, Stream Stream, string ContentType)
    {
        var region = RegionEndpoint.GetBySystemName(Region);

        var _client = new AmazonS3Client(region);

        var putRequest = new PutObjectRequest
        {
            BucketName = Bucketname,
            Key = Key,
            ContentType = ContentType,
            InputStream = Stream
        };

        _client.PutObjectAsync(putRequest).Wait();
    }

    public TemplateContext MakeTemplateContext()
    {
        var options = new TemplateOptions();
        options.MemberAccessStrategy = UnsafeMemberAccessStrategy.Instance;

        TemplateContext templateContext = new TemplateContext(options);
        AddFilters(options);

        // When a property of a JsonObjectvalue is accessed, try to look into its properties
//        options.MemberAccessStrategy.Register<JsonObject, object?>((source, name) => source[name]);

//        options.ValueConverters.Add(x => x is JsonObject o ? new ObjectValue(o) : null);
//        options.ValueConverters.Add(x => x is JsonValue v ? v.GetValue<object>() : null);

        return templateContext;
    }

    private void AddFilters(TemplateOptions TemplateOptions)
    {
        //TemplateOptions.Filters.AddFilter("replace-codepoint", Filters.ReplaceCodepoint);
        //TemplateOptions.Filters.AddFilter("replace-regex", Filters.ReplaceRegex);
        //TemplateOptions.Filters.AddFilter("set-filename", Filters.SetOutput);
        //TemplateOptions.Filters.AddFilter("indexof", Filters.IndexOfFilter);
    }


    public string GetFileContent(string Region, string Bucketname, string Key)
    {
        var region = RegionEndpoint.GetBySystemName(Region);

        var _client = new AmazonS3Client(region);

        var request = new GetObjectRequest();
        request.BucketName = Bucketname;
        request.Key = Key;

        GetObjectResponse response = _client.GetObjectAsync(request).Result;
        StreamReader reader = new StreamReader(response.ResponseStream);
        string content = reader.ReadToEnd();
        return content;
    }
}