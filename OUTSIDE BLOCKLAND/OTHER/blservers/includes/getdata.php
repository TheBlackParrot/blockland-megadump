<?php
	$serv = (isset($_GET['server'])) ? htmlspecialchars($_GET['server']) : 1;
	if(!ctype_digit($serv)) {
		http_response_code(400);
		die();
	}

	switch($serv) {
		case "1":
			$server = "master2.blockland.us";
			break;

		case "2":
			$server = "blmaster.theblackparrot.us";
			break;

		case "3":
			$server = "blmaster.hostoi.com";
			break;
	}

	$curl = curl_init();
	curl_setopt($curl, CURLOPT_URL, $server);
	curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);
	$data = curl_exec($curl);
	$data = str_replace("<", "&lt;", $data);
	curl_close($curl);

	header('Content-Type: text/plain');
	echo($data);