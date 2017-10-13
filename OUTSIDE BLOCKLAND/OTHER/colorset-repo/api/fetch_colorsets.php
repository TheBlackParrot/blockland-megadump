<?php
	$colorsetDir = dirname(dirname(__FILE__)) . "/colorsets";

	$iterator = new DirectoryIterator($colorsetDir);
	$return_arr = [];
	$count = 0;

	foreach ($iterator as $fileInfo) {
		if($fileInfo->isDot()) {
			continue;
		}
		if(!$fileInfo->isFile()) {
			continue;
		}
		if($fileInfo->getExtension() != "txt") {
			continue;
		}

		$filename = $fileInfo->getPathname();

		$currentColor = [];
		$currentSet = [];
		
		$data = file($filename, FILE_SKIP_EMPTY_LINES);
		foreach ($data as $lineNum => $line) {
			$firstWord = strtok($line, " ");

			if(stripos($firstWord, "DIV:") !== FALSE) {
				$columnName = trim(str_replace("DIV:", "", $line));
				$currentSet[$columnName] = $currentColor;
				$currentColor = [];
			} else {
				$col = explode(" ", trim($line));

				// i hate this
				if(stripos($col[0], ".") !== FALSE) {
					for($i=0; $i<count($col); $i++) {
						$col[$i] = ceil(255 * $col[$i]);
					}
				}

				$currentColor[] = array_slice($col, 0, 4);
			}
		}

		$return_arr[$fileInfo->getBasename('.txt')] = $currentSet;
		$count++;
	}

	$return_arr['COUNT'] = $count;

	die(json_encode($return_arr, JSON_NUMERIC_CHECK));
?>