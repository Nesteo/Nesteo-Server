module.exports = function (grunt) {
    'use strict';

    // Project configuration
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        // Sass compiling
        sass: {
            options: {
                implementation: require("node-sass"),
                sourceMap: false,
                outputStyle: 'compressed'
            },
            dist: {
                files: [
                    {
                        expand: true,
                        cwd: "Styles",
                        src: ["**/*.scss"],
                        dest: "wwwroot/css",
                        ext: ".css"
                    }
                ]
            }
        },

        // Copy files
        copy: {
            main: {
                files: [
                    {expand: true, flatten: true, src: ['node_modules/bootstrap/dist/js/bootstrap.bundle.min.js'], dest: 'wwwroot/js/'},
                    {expand: true, flatten: true, src: ['node_modules/jquery/dist/jquery.min.js'], dest: 'wwwroot/js/'}
                ]
            }
        },

        // Auto update
        watch: {
            files: ['Styles/**/*.scss'],
            tasks: ['sass', 'copy']
        }
    });

    // Load package tasks
    grunt.loadNpmTasks('grunt-sass');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-copy');

    // Register tasks
    grunt.registerTask('default', ['sass', 'copy']);
};
