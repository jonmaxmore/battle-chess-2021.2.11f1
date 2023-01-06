var str2unityStr = function(str) {
    let bufferSize = lengthBytesUTF8(str) + 1;
    let buffer = _malloc(bufferSize);
    return stringToUTF8(str, buffer, bufferSize);
};

mergeInto(LibraryManager.library, {
    w3u_initialize: function() {
        if (typeof web3 !== 'undefined'&&web3.currentProvider.selectedAddress!=null) {
            console.log("defined");
            web3 = new Web3(web3.currentProvider);
        } else {
            if (window.ethereum) {
                console.log("undefined-create");
                window.web3 = new Web3(window.ethereum);
        		window.ethereum.enable();
            }
            else alert("please connect metamask");
        }

    },
	w3u_connect:function (){
		window.ethereum.enable();
	},
    
    w3u_WalletAddress: function () {
    if(typeof web3 !== 'undefined'&&web3.currentProvider.selectedAddress!=null){
        console.log("defined");
        var returnStr;
        // get address from metamask
        returnStr = web3.currentProvider.selectedAddress;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
    else {
        return "false";
    }
    },

    w3u_isReady: function () {
        if (typeof web3 === undefined) 
            return false;
        return true;
    },

    w3u_getFirstAccount: function() {
        return str2unityStr(web3.eth.accounts[0]);
    },
    w3u_getAccount: function(idx) {
        return str2unityStr(web3.eth.accounts[idx]);
    },

    w3u_getBlockNumber: function() {
        return web3.eth.blockNumber;
    },

    w3u_sendFund: function(receiver, value){
        var sender = web3.eth.accounts[0];
        var receiver = Pointer_stringify(receiver);
        var amount = web3.toWei(value, "ether");

        web3.eth.sendTransaction({from:sender, to:receiver, value: amount}, function(err, transactionHash) {
            console.log(err);
            if (!err)
                console.log(transactionHash);
        });
    },
    w3u_getBalance: function(address) {
        return web3.eth.getBalance(Pointer_stringify(address));
    }
});
