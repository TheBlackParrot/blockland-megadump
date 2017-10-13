<?php
$cmd_str = "ffprobe -v quiet -print_format json -show_format -show_streams";

header("Content-type: text/plain");

echo "START\r\n";

$files = array();

$fileIterator = new DirectoryIterator(__DIR__ . "/music");
foreach($fileIterator as $fileInfo) {
	if(!$fileInfo->isFile()) {
		continue;
	}

	if($fileInfo->getExtension() != "ogg") {
		continue;
	}

	$filename = $fileInfo->getFilename();
	$files[$filename] = $fileInfo->getPathname();
}

ksort($files);

foreach($files as $filename => $pathname) {
	$esc = escapeshellarg($pathname);
	$file_data = json_decode(shell_exec("$cmd_str $esc"), true);
	$tags = $file_data["streams"][0]["tags"];
	$duration = ceil($file_data["streams"][0]["duration"]*1000);
	$url_part = "music/" . rawurlencode($filename);

	if(array_key_exists("TITLE", $tags)) {
		echo "{$tags["TITLE"]}\t";
	} else if(array_key_exists("NAME", $tags)) {
		echo "{$tags["NAME"]}\t";
	}
	echo "{$tags["ARTIST"]}\t";
	echo "$duration\t";
	echo "$url_part";
	echo "\r\n";
}

echo "END\r\n";