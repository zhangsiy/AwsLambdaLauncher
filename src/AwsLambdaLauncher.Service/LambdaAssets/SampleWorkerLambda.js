console.log('Loading function');
var AWS = require('aws-sdk');

exports.handler = function (event, context) {
    var eventText = 'Event Data Looks Like:' + JSON.stringify(event, null, 2);
    console.log('Received event:', eventText);
    var sns = new AWS.SNS();
    var params = {
        Message: eventText,
        Subject: 'Test SNS From Lambda',
        TopicArn: 'arn:aws:sns:us-east-1:119381170469:Temp_Jeff_Test_Lambda_Output_Topic'
    };
    sns.publish(params, context.done);
};