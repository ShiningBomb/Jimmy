# Stair2Slide
An AI optimization helper library.

Many optimization algorithms are built for continous numerical parameters. It is non-trivial to define text as parameter, or a list of options to choose from, or complex formulas (eg. one parameter that tied with another), with Stair2Slide you can have all of those without changing any of your optimization algorithm.

### Main features ###
* Easily use candidate values and formulas instead of min, max, and resolution for each parameter.
* Provide interface for firing progress and results to the UI, so user don't have to wait at blank screen.
* Provide interface for when optimization cancels in the UI, so it can terminate soon.
* Prevent duplicate evaluation so you have one less thing to think about.
