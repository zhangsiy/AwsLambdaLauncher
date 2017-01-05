## Tech Notes

* Setup Lambda function
  * Need a role to be defined, that can access Lambda and the source to trigger the function (such as S3), and any other AWS access needed (such as if you want Lambda to publish an event to SNS, the role needs access to SNS)
* Create a subscription to the SNS topic, such as another Lambda or an end point or a SQS queue to allow async processing of the lambda output.
  * Sample code to publish to SNS from Lambda: http://stackoverflow.com/questions/31484868/can-you-publish-a-message-to-an-sns-topic-using-an-aws-lambda-function-backed-by
* AWS SDK for Asp.Net Core
  * Require to configure credentials via profile files
    *  C:\Users\{username}\.aws\credentials
  * The proposed solution to use the extension package "AWSSDK.Extensions.NETCore.Setup" does not work when targeting 4.5.2
  * Have to read the configuration section out and construct
    * StoredProfileAWSCredentials as AWSCredentials object needed
    * RegisonEndpoint as aws endpoint object needed
  * Then Can use those to fulfill the constructors such as AmazonS3Client
* Setup a Lambda repo management facility
  * Setup a S3 bucket to hold all Lambda deployment packages
    * Each is a .zip file contains the files per Lambda requirements for specific language. E.g. for JS, the ZIP file should contain a index.js file which contains the code.
  * Setup a Lambda function that
    * is triggered by an update to an object in the S3 bucket above
    * update or create Lambda functions with the event data
      * See https://aws.amazon.com/blogs/compute/new-deployment-options-for-aws-lambda/
    * Need to do all of the following
      * Able to use lambda SDK to check existense of the function (getFunction)
      * Able to use lambda SDK to update function code if already exists (updateFunctionCode)
      * Able to use lambda SDK to create function if none exists (createFunction)
      * Able to use lambda SDK to attach the right permission to the newly created function (addPermission)
      * Able to use SNS SDK to subscribe the newly created function to the SNS topic it needs to observe (sns.subscribe). 
        * Or other services in a similar fashsion.
      * Setup reference document: https://aws.amazon.com/blogs/mobile/invoking-aws-lambda-functions-via-amazon-sns/
* CC Distributed Attribute Calculation Architecture Diagram
  * https://www.lucidchart.com/invitations/accept/bdbd9b1d-ade8-4849-8719-87ae8b34cd60
* S3 events to SNS
  * Open up permissions on SNS
    * http://docs.aws.amazon.com/AmazonS3/latest/dev/NotificationHowTo.html#grant-destinations-permissions-to-s3
  * Configure on S3 bucket
    * On "Properties", go to Events
    * Add the right events for the S3 to notify about set of operations. such as notifying SNS topic about S3 object updated/created/deleted.
* Package up code for Lambda Deployment (JS code)
  * AWS Lambda has very specific requirement for the structure of code deployment package. 
  * E.g. for NodeJS based functions
    * Must be a .zip file
    * The entry .js file (e.g. index.js) lives at the root level of the ZIP file
    * All npm dependencies need to be pre-downloaded, and included into the ZIP file, under the "node_modules" folder, which is sitting at the root level of the ZIP file. 
  * For AspNet Core, use ZipArchive to produce such a ZIP file stream in memory. 
    * Need reference to "System.IO.Compression.FileSystem"
  * For npm modules, need to pre-download them, and make sure they are included as part of the AspNet Core publish artifacts, so the runtime can reference them to produce the complete Lambda code package. 
    * Make sure that in the code package producing logic, to recursively iterate through the folder and add all files to the ZipArchive, while maintaining the original folder structure.    