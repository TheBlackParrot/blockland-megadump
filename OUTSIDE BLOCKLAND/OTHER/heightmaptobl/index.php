<?php
	$root = dirname(__FILE__);

	header("Content-Type: text/plain");

	$image = new Imagick();
	$filename = "$root/map.jpg";

	$image->readImage($filename);
	if($image->valid()) {
		if(isset($_GET['resize'])) {
			$image->resizeImage($_GET['resize'], $_GET['resize'], imagick::FILTER_LANCZOS, 1);
		}
		echo($image->getImageWidth() . "\t" . $image->getImageHeight() . "\n");

		for($x = 1; $x <= $image->getImageWidth(); $x++) {
			for($y = 1; $y <= $image->getImageHeight(); $y++) {
				$pixel = $image->getImagePixelColor($x, $y);
				$color = $pixel->getColor();

				$overall = ($color['r'] + $color['g'] + $color['b'])/(255*3)*70;
				echo(($x-1) . "\t" . ($y-1) . "\t" . "$overall\n");
			}
		}
	}
?>