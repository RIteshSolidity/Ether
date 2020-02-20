
var Web3 = require("web3")
var web3 = new Web3("http://127.0.0.1:8080")

var solc = require("solc")
var fs = require("fs")
var code  = fs.readFileSync("C:\\test\\first.sol", 'UTF-8')

var input = {
    language: 'Solidity',
    sources: {
        'first.sol': {
            content: code
        }
    },
    settings: {
        outputSelection: {
            '*': {
                '*': [ '*' ]
            }
        }
    }
}

var output = JSON.parse(solc.compile(JSON.stringify(input)));

output.contracts['first.sol'].testcontract.abi
output.contracts['first.sol'].testcontract.evm.bytecode.object

var abi = output.contracts['first.sol'].testcontract.abi
var bytecode = output.contracts['first.sol'].testcontract.evm.bytecode.object
var firstContract = new web3.eth.Contract(abi)

var sender = '0x173B8b6750377790555F07eF45309e826ab9eDe2'
web3.eth.personal.unlockAccount(sender,'riteshmodi', 15000).then(console.log)
firstContract.deploy({data: '0x' + bytecode, arguments:[]}).send({from:sender, gas: 3000000}).on('receipt', function(r){ console.log(r)  })

var contractAddress = '0xE5134611D493F2DaD1d51FDcA4644084b2E82529'

var client = new web3.eth.Contract(abi, contractAddress)


 client.methods.getName().call().then(console.log)

 client.methods.setName("ritesh modi").send({from: sender, gas: 3000000}).on('receipt', function(r){ console.log(r)  })

 client.methods.getName().call().then(console.log)
