#!/usr/bin/env perl
# This project is a place holder for a Perl version of the XML editor. The idea
# is to automatically generate a GUI capable of editing an XML file from an XSD
# describing the data. 
#
# This project should eventually be turned into a module, or a Perl/Tk widget
# (if possible). An example program should be produced (which could be turned
# into a standalone executable on Windows) that uses this module.

use warnings;
use strict;

use Getopt::Long; # http://perldoc.perl.org/Getopt/Long.html
use Data::Dumper; # http://perldoc.perl.org/Data/Dumper.html
use Tk; # http://www.perlmonks.org/?node_id=922840
# XML parser?

# TODO Implement all this junk

my $mw = MainWindow->new;
$mw->Button(-text => "DXSDE Perl", -command => sub{ exit })->pack;
MainLoop;

