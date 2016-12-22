console.log('Loading function');
var AWS = require('aws-sdk');
var lambda = new AWS.Lambda();
var sns = new AWS.SNS();

var configurations = {
    codeRepoS3BucketName: "temp-jeff-test-attribute-calculation-lambda-bucket1",
    functionHandler: "index.handler",
    roleArn: "arn:aws:iam::119381170469:role/service-role/jeffTestLambdaRole",
    codeRuntime: "nodejs4.3",
    defaultMemorySize: 128,
    defaultTimeout: 10,
    snsTopicArn: "arn:aws:sns:us-east-1:119381170469:Temp_Jeff_Test_S3_To_SNS"
};

exports.handler = function (event, context) {
    key = event.Records[0].s3.object.key
    bucket = event.Records[0].s3.bucket.name
    version = event.Records[0].s3.object.versionId
    if (bucket == configurations.codeRepoS3BucketName && key.endsWith('.zip')) {
        var extIndex = key.lastIndexOf(".zip");
        var functionName = key.substring(0, extIndex);

        console.log("check if the lambda function already exists: " + functionName);
        var checkParams = {
            FunctionName: functionName
        };
        lambda.getFunction(checkParams, function (err) {
            if (err) {
                if (err.statusCode == '404') {
                    // Function not yet exists, create it
                    var createParams = {
                        FunctionName: functionName,
                        Handler: configurations.functionHandler,
                        Role: configurations.roleArn,
                        Runtime: configurations.codeRuntime,
                        Code: {
                            S3Bucket: bucket,
                            S3Key: key,
                            S3ObjectVersion: version
                        },
                        Timeout: configurations.defaultTimeout,
                        MemorySize: configurations.defaultMemorySize,
                        Publish: true
                    };
                    lambda.createFunction(createParams, function (creareFunctionError, creareFunctionData) {
                        if (creareFunctionError) {
                            console.log(creareFunctionError, creareFunctionError.stack);
                            context.fail(creareFunctionError);
                        } else {
                            console.log(creareFunctionData);
                            // Now add permission to allow sns invocation to this newly created function
                            var permissionParam = {
                                Action: 'lambda:invokeFunction',
                                FunctionName: creareFunctionData.FunctionArn,
                                Principal: 'sns.amazonaws.com',
                                StatementId: 'default',
                                SourceArn: configurations.snsTopicArn
                            };
                            lambda.addPermission(permissionParam, function (addLambdaPermissionError, addLambdaPermissionData) {
                                if (addLambdaPermissionError) {
                                    console.log(addLambdaPermissionError, addLambdaPermissionErrorsubscribeError.stack);
                                    context.fail(addLambdaPermissionError);
                                } else {
                                    console.log(addLambdaPermissionData);
                                    context.succeed(addLambdaPermissionData);
                                }
                            });

                            // Now try create the subscription from the new lambda to the SNS topic
                            var subscribeParams = {
                                Protocol: 'lambda',
                                TopicArn: configurations.snsTopicArn,
                                Endpoint: creareFunctionData.FunctionArn
                            };
                            sns.subscribe(subscribeParams, function(subscribeError, subscribeData) {
                                if (subscribeError) {
                                    console.log(subscribeError, subscribeError.stack);
                                    context.fail(subscribeError);
                                } else {
                                    console.log(subscribeData);
                                    context.succeed(subscribeData);
                                }
                            });
                        }
                    });
                } else {
                    // Other unexpected failures, log and bump out
                    console.log(err, err.stack);
                    context.fail(err);
                }
            } else {
                // Function exists, update with the new code
                console.log("uploaded to lambda function: " + functionName);
                var updateParams = {
                    FunctionName: functionName,
                    S3Key: key,
                    S3Bucket: bucket,
                    S3ObjectVersion: version
                };
                lambda.updateFunctionCode(updateParams, function (error, data) {
                    if (error) {
                        console.log(error, error.stack);
                        context.fail(error);
                    } else {
                        console.log(data);
                        context.succeed(data);
                    }
                });
            }
        });
    } else {
        context.succeed("skipping zip " + key + " in bucket " + bucket + " with version " + version);
    }
};