# Androdev [![Build Status](https://travis-ci.org/fahminlb33/Androdev.svg?branch=master)](https://travis-ci.org/fahminlb33/Androdev)
Enhaced Android development environment. Designed to participate in PSN STEM IPB.

## Purpose
The main purpose of this project is to create a simple installer for Eclipse Mars 2 to work with Android Developer Tools. I've seen many people having trouble to configure Eclipse with ADT for Android development.

Using this installer, people can get a fully working Eclipse IDE with Android Developer Tools (plus Android SDK). This app is divided into two version, *Web installer* and *Standalone installer*. Standalone version of Androdev is not availiable to download, but you must create it yourself (see Wiki).

## Features
* Automate installation process of Java Development Kit, Android SDK, and Eclipse IDE.
* Has auto-configurator for Eclipse Code Assist for better coding experience.
* Well coded by Indonesian student :clap:

### Androdev Web Installer
Androdev web installer version is very lightweight. Just run Androdev and clik **Update**. Androdev will download necessary packages and then install them onto your system. But web installer doesn't provide *ready-to-use* environment. You need to download necessary packages in Android SDK Tools like SDK Platforms or System Images before using Eclipse IDE.

### Androdev Standalone
Androdev standalone version includes these following packages:

* Java Development Kit 8 Update 101 32-bit
* Eclipse Mars 2 32-bit
* Android Developer Tools 23.0.7
* Android SDK Tools complete packages

Android SDK Tools installed package list.

Package|Revision
-------|-------
Android SDK Tools|24.1.2
Android SDK Platform-tools|24
Android SDK Build-tools|23.0.1-3
SDK Platform|API 16-24
Android Support Repository|36
Google Play Services|32
Google Repository|32
Google USB Driver|11

## Project status
* This project is still **work in process**.
* ~~Nothing has tested.~~ Just a little part of code has been tested.
* Need more unit tests.
* Travis CI build will ALWAYS failing. Because of "IWshRuntimeLibrary" interface is not availiable in Docker.
