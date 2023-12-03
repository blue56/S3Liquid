using Amazon.Lambda.Core;
using Fluid;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3Liquid;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(S3LiquidRequest Request, ILambdaContext context)
    {
        Liquid liquid = new Liquid();
        liquid.Run(Request);

        return "OK";
    }
}
