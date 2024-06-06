<?php
/*
 * IPWorks IoT 2024 PHP Edition - Sample Project
 *
 * This sample project demonstrates the usage of IPWorks IoT in a 
 * simple, straightforward way. It is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * www.nsoftware.com/ipworksiot
 *
 * This code is subject to the terms and conditions specified in the 
 * corresponding product license agreement which outlines the authorized 
 * usage and restrictions.
 */
require_once('../include/ipworksiot_amqp.php');
require_once('../include/ipworksiot_const.php');
?>
<?php
	class MyAMQP extends IPWorksIoT_AMQP
	{
		function fireSSLServerAuthentication($param) {
			$param['accept'] = true;
			return $param;
		}
	}

	$amqp = new MyAMQP();
?>

<form method="post">
	<table>
	<tr><td>Host</td><td><input type="text" name="host" value="<?php echo isset($_POST["host"])?$_POST["host"]:""; ?>" size=35></td></tr>
	<tr><td>Port</td><td><input type="text" name="port" value="<?php echo isset($_POST["port"])?$_POST["port"]:"5672"; ?>" size=35></td></tr>
	<tr><td>User</td><td><input type="text" name="user" value="<?php echo isset($_POST["user"])?$_POST["user"]:""; ?>" size=35></td></tr>
	<tr><td>Password</td><td><input type="password" name="password" value="<?php echo isset($_POST["password"])?$_POST["password"]:""; ?>" size=35></td></tr>
	<tr><td>Enable SSL</td><td><input type="checkbox" name="ssl" value="<?php echo isset($_POST["ssl"])?$_POST["ssl"]:"1"; ?>" size=35></td></tr>
	<tr><td>Message</td><td><input type="text" name="message" value="<?php echo isset($_POST["message"])?$_POST["message"]:"Hello, world!"; ?>" size=35><input type="submit" name="send" value="Send Message"></td></tr>
	<tr><td><input type="submit" name="retrieve" value="Retrieve Message"></td><td></tr>
	</table>
	</form>

<?php

if ($_SERVER['REQUEST_METHOD'] == "POST") {
	try {
		//$amqp = new IPWorksIoT_AMQP();
		$amqp->setUser($_POST["user"]); //set user and password from form
		$amqp->setPassword($_POST["password"]);
		$amqp->setContainerId("ContainerId"); //set unique container id
		$amqp->setSSLEnabled(isset($_POST['ssl']) && $_POST['ssl']); //set ssl enabled or disabled according to check box
		$amqp->doConnectTo($_POST["host"],$_POST["port"]); //connect with host and port provided in form
		$amqp->doCreateSession("SessionName"); //create session, senderlink and receiverlink
		$amqp->doCreateSenderLink("SessionName","SenderLinkName","TargetName");
		$amqp->setReceiveMode(1); //0 for automatic, 1 for retrieve
		$amqp->doCreateReceiverLink("SessionName","ReceiverLinkName","TargetName");
		
		if(isset($_POST['send'])) {
			//send message
			$amqp->doResetMessage();
			$amqp->setMessageValueType(17); //string type
			$amqp->setMessageValue($_POST["message"]);
			$amqp->doSendMessage("SenderLinkName");
			echo "Message sent.";
		} else if(isset($_POST['retrieve'])) {
			//retrieve message
			$amqp->setRetrieveTimeout(5); //5 second timeout
			echo "Retrieveing Message... ";
			$amqp->doRetrieveMessage("ReceiverLinkName");
			echo "Message received: ",$amqp->getReceivedMessageValue();
		}
	} catch (Exception $e) {
		echo 'Error: ', $e->getMessage(), "\n";
	}
}
?>
