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
require_once('../include/ipworksiot_mqtt.php');
require_once('../include/ipworksiot_const.php');
?>
<?php
	global $receivedMessage;
	$receivedMessage = null;
	class MyMQTT extends IPWorksIoT_MQTT
	{
		// Fires when a message is received
		function fireMessageIn($param) {
			global $receivedMessage;
			$receivedMessage = $param;
		}
	}

	$mqtt = new MyMQTT();

	// Get connection variables from form or use defaults
	$host= (empty($_POST['host'])) ? 'test.mosquitto.org' : $_POST['host'];
	$port= (empty($_POST['port'])) ? '1883' : $_POST['port'];
	$topic= (empty($_POST['topic'])) ? 'nsoftware/test2' : $_POST['topic'];
	$qos= (empty($_POST['qos'])) ? '0' : $_POST['qos'];
	$message = (empty($_POST['message'])) ? 'Hello World!' : $_POST['message'];
?>

	<form method="post">
	<table>
	<tr><td>Host</td><td><input type="text" name="host" value="<?php echo $host; ?>" size="35"></td></tr>
	<tr><td>Port</td><td><input type="text" name="port" value="<?php echo $port; ?>" size="35"></td></tr>
	<tr><td>Topic</td><td><input type="text" name="topic" value="<?php echo $topic; ?>" size="35"></td></tr>
	<tr><td>QoS</td><td><input type="text" name="qos" value="<?php echo $qos; ?>" size="35"></td></tr>
	<tr><td>Message</td><td><input type="text" name="message" value="<?php echo $message; ?>" size="35"></td></tr>
	<tr><td></td><td><input type="submit" value="Publish Message"></td></tr>
	</table>
	</form>

<?php

if ($_SERVER['REQUEST_METHOD'] == "POST") {

	try{
		// Connect, subscribe to the topic, and publish the message
		$mqtt->doConnectTo($host, $port);
		$mqtt->doSubscribe($topic, $qos);
		$mqtt->doPublishMessage($topic, $qos, $message);

		// Wait for a message to be received
		global $receivedMessage;
		while($receivedMessage == null)
		{
			$mqtt->doEvents();
		}
		
		// Display the received message's details
		echo "<hr><b>Received from " . $receivedMessage['topic'];
		echo " at QoS " . $receivedMessage['qos'];
		if ($receivedMessage['retained']) {
		echo " [Retained message]";
		}
		echo ": ";
		echo $receivedMessage['message'] . "</b>";
		$mqtt->doDisconnect();
	} catch (Exception $e) {
	echo 'Error: ', $e->getMessage(), "\n";
	}
}
?>
