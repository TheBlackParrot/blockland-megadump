<?php
	$rules_filename = dirname(__FILE__) . "/rules.json";
	$rules_raw_data = file_get_contents($rules_filename);
	$rules = json_decode($rules_raw_data, true);

	$terms_filename = dirname(__FILE__) . "/terms.json";
	$terms_raw_data = file_get_contents($terms_filename);
	$terms = json_decode($terms_raw_data, true);
?>

<html>

<head>
	<link rel="stylesheet" type="text/css" href="css/reset.css">
	<link rel="stylesheet" type="text/css" href="css/main.css">

	<script type="text/javascript" src="js/jquery.js"></script>
</head>

<body>
	<div class="wrapper">
		<h1>Jailbreak</h1>
		<hr/>
		<?php
			// loop through each rule section
			foreach($rules as $section_name => $data) {
				// section name is the key, data is everything within it

				// create a section element, set the anchor id
				echo '<div class="section" id="' . $data['divID'] . '">';

					// if we have an image variable set, add it to a left floated element
					if(isset($data['image'])) { 
						echo '<div class="left">';
						echo '<img src="images/' . $data['image'] . '"/>';
						echo '</div>';
					}

					// now to generate the rule part of the section
					echo '<div class="right">';
					echo '<h2>' . $section_name . '</h2>';

					// loop through the array of rules, $i is the index, $rule_parts contains the rule/extra parts
					foreach($data['rules'] as $i => $rule_parts) {
						$i++;
						echo '<div class="rule" id="' . $data['divID'] . '-' . $i . '">';
						echo "<strong>$i.</strong> ";
						echo $rule_parts['rule'];
						
						// check to see if we defined any specific extra rule parts
						// $rule_parts['extras'] will be a regular array of strings, we can just foreach it and echo out values
						if(isset($rule_parts['extras'])) {
							foreach($rule_parts['extras'] as $j => $extra_part) {
								// $j can be left out of a foreach of an array, it's just there so we have a list number
								// fun fact: we can use chr with an offset to automatically get the letter! (see an ascii table)
								$j += 97;
								$chr = chr($j);

								echo '<div class="rule-extra" id="' . $data['divID'] . '-' . $i . $chr . '">';
								echo "<strong>$i$chr.</strong> ";
								echo $extra_part;
								echo '</div>';
							}
						}
						echo '</div>';
					}
					echo '</div>';
				echo '</div>';
			}

			// now to generate terms, these are stored separate from rules as they're not rules
			echo '<div class="section" id="terms">';
			echo '<h2>Terminology</h2>';
			// foreach is always key => value, but you're free to set variable names
			$i = 0;
			foreach($terms as $term => $definition) {
				$i++;

				echo '<div class="term" id="term-' . $i . '">';
					echo '<span class="term-name">' . $term . '</span>';
					echo '<span class="term-def">' . $definition . '</span>';
				echo '</div>';
			}
			echo '</div>';

			// PHP comments are hidden, only those with access to the script itself can see them
		?>
	</div>

	<script type="text/javascript" src="js/main.js"></script>
</body>

</html>