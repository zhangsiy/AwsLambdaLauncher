console.log('Loading function');
var AWS = require('aws-sdk');
var Xpath = require('xpath');
var DomParser = require('xmldom').DOMParser;

exports.handler = function (event, context) {
    var eventText = JSON.stringify(event, null, 2);
    console.log('Received event:', eventText);

    // Parse out the target data
    var messageText = event['Records'][0]['Sns']['Message'];
    var parsedMessage = JSON.parse(messageText);
    var xml = parsedMessage['Xml'];

    // Sample XML parsing for attribute
    var xmlDoc = new DomParser().parseFromString(xml);
    var imageNodes = Xpath.select('//img', xmlDoc);

    // Construct output
    var outputMessage = 'No images found';
    if (imageNodes.length > 0) {
        outputMessage = 'Found ' + imageNodes.length + ' images';
    }

    var sns = new AWS.SNS();
    var params = {
        Message: outputMessage,
        Subject: 'Test template XML processing Lambda',
        TopicArn: 'arn:aws:sns:us-east-1:119381170469:Temp_Jeff_Test_Lambda_Output_Topic'
    };
    sns.publish(params, context.done);
};