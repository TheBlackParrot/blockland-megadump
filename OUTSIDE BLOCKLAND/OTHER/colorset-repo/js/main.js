console.log("This is just me practicing things");

var colorsetData;

function fetchColorsetData(callback) {
	var url = 'api/fetch_colorsets.php';

	$.ajax({
		type: 'GET',
		url: url,
		contentType: 'text/plain',
		dataType: 'json',
		xhrFields: {
			withCredentials: false
		},
		success: function(data) {
			colorsetData = data;

			if(typeof callback === "function") {
				callback(data);
			}
		},
		error: function() {
			console.log("error");
		}
	});
}

function addColorsetPreview(colorSet, name) {
	if(name == "COUNT") {
		console.log(colorSet + ' colorsets should be rendered.');
		return;
	}

	console.log("Rendering preview for " + name + "...");

	var element = $('<div></div>');

	element.addClass("preview").addClass("colorset");
	element.attr("colorset", name);

	element.append('<div class="card_head">' + name + '</div>');

	for(var columnName in colorSet) {
		var column = colorSet[columnName];
		//console.log(column);

		element.append(renderColorsetColumn(column, columnName));
	}

	$(".selection").append(element);
}

function renderColorsetColumn(column, columnName) {
	var domColumn = $("<div></div>");
	
	domColumn.addClass("col");
	domColumn.attr("column", columnName);

	for(var colorRow in column) {
		var colorData = column[colorRow];
		if(colorData.length <= 1) {
			continue;
		}

		colorData[3] /= 255;

		var domColumnCell = $('<div></div>');
		domColumnCell.addClass("cell");
		domColumnCell.attr("style", 'background: linear-gradient(rgba(' + colorData.join(",") + '), rgba(' + colorData.join(",") + ')), url(\'images/trans.png\') no-repeat center;');

		domColumn.append(domColumnCell);
	}

	return domColumn;
}

fetchColorsetData(function(data) {
	for(var colorSetName in data) {
		var colorSet = data[colorSetName];
		//console.log(colorSet);

		addColorsetPreview(colorSet, colorSetName);
	}
});