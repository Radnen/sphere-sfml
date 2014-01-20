/**
* Script: testsuite.js
* Written by: Andrew Helenius
* Updated: 12/22/2013
**/

function TestSuite()
{
	this.tests = [];
}

TestSuite.prototype.addTest = function(name, func, times) {
	this.tests.push({name: name, func: func, times: times});
}

TestSuite.prototype.run = function() {
	var printout = "";
	for (var t = 0; t < this.tests.length; ++t) {
		var test = this.tests[t];
		var trials = [];
		for (var i = 0; i < test.times; ++i) {
			var time = GetTime();
			test.func();
			trials[i] = GetTime() - time;
		}
		printout += test.name + ": " + this.avg(trials) + "ms\n";
	}
	return printout;
}

TestSuite.prototype.avg = function(array) {
	if (array.length == 0) return 0;
	var avg = 0;
	for (var i = 0; i < array.length; ++i) avg += array[i];
	return avg / array.length;
}

TestSuite.prototype.toString = function() { return this.printout; }

// fastest:
// using new Array() with array[i]
// postfix ++ in old sphere
// 

var Loops = new TestSuite();

Loops.addTest("traditional for", function() {
}, 25);

/*Loops.addTest("For i++ in Check", function() {
	for (var i = 0; i++ < 10000000;) { }
}, 10);*/

/*Loops.addTest("For i++ in Step", function() {
	for (var i = 0; i < 10000000; i++) { }
}, 10);

Loops.addTest("For i++ in Body", function() {
	for (var i = 0; i < 10000000;) { i++; }
}, 10);

Loops.addTest("For i += 1 in Step", function() {
	for (var i = 0; i < 10000000; i += 1) { }
}, 10);

Loops.addTest("For i += 1 in Body", function() {
	for (var i = 0; i < 10000000;) { i += 1; }
}, 10);

Loops.addTest("For i += 2 in Step", function() {
	for (var i = 0; i < 10000000; i += 2) { }
}, 10);

Loops.addTest("For i++ in Body and Step", function() {
	for (var i = 0; i < 10000000; i++) { i++; }
}, 10);

Loops.addTest("While i++ in Step", function() {
	var i = 0;
	while (i++ < 10000000) { }
}, 10);

Loops.addTest("While i++ in Body", function() {
	var i = 0;
	while (i < 10000000) { i++; }
}, 10);*/

function game() { Abort(Loops.run()); }