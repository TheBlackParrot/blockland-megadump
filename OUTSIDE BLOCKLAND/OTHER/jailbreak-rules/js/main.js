var anchor = window.location.hash.replace("<", "&lt;");
console.log("Anchor value: " + anchor);

if(anchor) {
	$(anchor).css("background-color", "#ff8");
}

$("body").on("click", function(event){
	event.stopPropagation();

	if(history.pushState) {
		history.pushState(null, null, "#");
	}
	else {
		location.hash = "#";
	}

	$(".rule, .term, .rule-extra").each(function(){
		$(this).removeAttr("style");
	});
});

$(".rule, .term, .rule-extra").on("click", function(event){
	event.stopPropagation();
	newHash = "#" + $(this).attr("id");

	if(history.pushState) {
		history.pushState(null, null, newHash);
	}
	else {
		location.hash = newHash;
	}

	$(".rule, .term, .rule-extra").each(function(){
		$(this).removeAttr("style");
	});
	$(newHash).css("background-color", "#ff8");
});