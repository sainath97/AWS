using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace CopyObjectsFromSourceUsingLambdaCron
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(ILambdaContext context)
        {
            IAmazonS3 s3Client = new AmazonS3Client();
            var sourceBucket = Environment.GetEnvironmentVariable("sourceBucket");
            var destinationBucket = Environment.GetEnvironmentVariable("destinationBucket");
            if (sourceBucket!=null && await s3Client.DoesS3BucketExistAsync(sourceBucket))
            {
                var listObjectResponse = await s3Client.ListObjectsAsync(new ListObjectsRequest
                {
                    BucketName = sourceBucket
                });
                if (listObjectResponse != null)
                {
                    var objectsList = listObjectResponse.S3Objects;
                    if (objectsList.Any())
                    {
                        objectsList.ForEach(obj => {
                            LambdaLogger.Log("Copying Object: " + obj.Key);
                            s3Client.CopyObjectAsync(sourceBucket, obj.Key, destinationBucket, 
                                obj.Key.Split('.')[0] + "_copiedFromCronJob." + obj.Key.Split('.')[1]);
                        });
                    }
                }
            }
        }
    }
}
