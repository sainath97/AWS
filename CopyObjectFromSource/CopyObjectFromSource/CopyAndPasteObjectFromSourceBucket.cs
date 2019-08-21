using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CopyObjectFromSource
{
    public class CopyAndPasteObjectFromSourceBucket
    {
        private readonly IAmazonS3 _s3Client;

        public CopyAndPasteObjectFromSourceBucket(IAmazonS3 s3Clinet)
        {
            _s3Client = s3Clinet;
        }

        public async Task CopyAndPasteObject(S3Event evnt)
        {
            var record = evnt.Records[0];
            using (var objectResponse = await _s3Client.GetObjectAsync(record.S3.Bucket.Name, record.S3.Object.Key))
            using (Stream responseStream = objectResponse.ResponseStream)
            {
                LambdaLogger.Log($"Uploading image Started {record.S3.Bucket.Name}:{record.S3.Object.Key}");

                // Upload the copied image back to S3
                await _s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = "mycopybucket.awsincubation",
                    Key = record.S3.Object.Key + "_Copy",
                    InputStream = responseStream,
                    Headers =
                    {
                        ContentLength=responseStream.Length
                    }
                });
                LambdaLogger.Log($"Uploading image Completed mycopybucket.awsincubation:{record.S3.Object.Key}" + "_Copy");
            }
        }

    }
}
