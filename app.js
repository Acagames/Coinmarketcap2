var request = require('request')
var myObj;
const http = require('http');
const url = require('url');
const port = 6006;
var endparse;
const requestHandler =(request,respone)=>{
    CoinCap();
    if (endparse)
    respone.end(JSON.stringify(myObj))
}

function CoinCap() {

    var url = "https://api.coinmarketcap.com/v1/ticker/?limit=100"

    request({
        url: url,
        json: true
    }, function (error, response, body) {

        if (!error && response.statusCode === 200) {
            console.log(body) // Print the json response
        }
        myObj= body;
        endparse =true;
        console.log(`endparse:= ${endparse}`);
    })

};

const server = http.createServer(requestHandler);

server.listen(port,(err) =>{
    if(err){
        return console.log(`Err ${err}`);
    }
    console.log(`List on port: ${port}`);
});


