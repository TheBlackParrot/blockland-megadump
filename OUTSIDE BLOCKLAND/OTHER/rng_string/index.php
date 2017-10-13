<?php
	header("Content-type: text/plain");

	function clamp($num, $min, $max) {
		return max($min, min($max, $num));
	}

	$valid_types = [
		"alnum",
		"digit",
		"alpha",
		"lwr-alnum",
		"lwr-alpha",
		"hir-alnum",
		"hir-alpha",
		"symbols",
		"hex",
		"hir-hex",
		"oct",
		"bin"
	];

	$lwr = implode("", range("a", "z"));
	$hir = strtoupper($lwr);
	$nums = implode("", range(0, 9));
	$syms = '*_-+=.!?:$%#^@/&,|";\'\\()[]{}<>';
	$hex = implode("", range(0, 9)) . implode("", range("a", "f"));

	$chars = [
		"alnum" => $lwr . $hir . $nums,
		"digit" => $nums,
		"alpha" => $lwr . $hir,
		"lwr-alnum" => $lwr . $nums,
		"lwr-alpha" => $lwr,
		"hir-alnum" => $hir . $nums,
		"hir-alpha" => $hir,
		"symbols" => $syms,
		"hex" => $hex,
		"hir-hex" => strtoupper($hex),
		"oct" => implode("", range(0, 7)),
		"bin" => "01"
	];

	$type = "alnum";
	$selected_chars = $chars[$type];

	if(isset($_GET['type'])) {
		if(in_array($_GET['type'], $valid_types)) {
			$type = $valid_types[array_search($_GET['type'], $valid_types)];
			$selected_chars = $chars[$type];
		}
	}
	if($type != "symbols" && isset($_GET['add_symbols'])) {
		if($_GET['add_symbols'] && ctype_digit($_GET['add_symbols'])) {
			$selected_chars .= substr($syms, 0, clamp($_GET['add_symbols'], 1, strlen($syms)));
		}
	}

	function generateRandomString($chars, $length) {
		$ret = "";

		for($i = 0; $i < $length; $i++) {
			$ret .= $chars[mt_rand(0, strlen($chars)-1)];
		}

		return $ret;
	}

	$amount = 1;
	if(isset($_GET['amount'])) {
		if(ctype_digit($_GET['amount'])) {
			$amount = clamp($_GET['amount'], 1, 250);
		}
	}

	$length = 10;
	if(isset($_GET['length'])) {
		if(ctype_digit($_GET['length'])) {
			$length = clamp($_GET['length'], 1, 150);
		}
	}

	$returns = [
		"generated" => [],
		"length" => $length,
		"amount" => $amount,
		"chars" => $selected_chars,
		"timestamp" => time()
	];

	$start = microtime();
	for($i = 0; $i < $amount; $i++) {
		$returns["generated"][] = generateRandomString($selected_chars, $length);
	}

	$returns["generation_time"] = microtime() - $start;

	die(json_encode($returns, JSON_PRETTY_PRINT));
?>