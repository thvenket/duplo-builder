# Clone the Repo and replace <OrgName> with your company name

# ZBuilder Images

Kitkat runs builds in a docker image. Different builds might require different
images to be able to compile, find dependencies, run tests, etc. This directory
is a way to create those custom images.

## Image generation

If you want to generate an image for one of the currently available languages,
cd into this directory and run

```
./build-image.sh yourLanguage
```

This will take a while and after that you will have a new image named
<OrgName>/zbuilder-yourLanguage:latest in your machine. If you want, you can
push that image to docker hub. We are not doing this automatically in case
you want to review the image before pushing.

### Regenerate all images

If you run

```
./build-image.sh all
```

images for all available languages will be generated. It could take a while if
you don't have them already in your base system.

### Why would I want to regenerate an ZBuilder image?

- Because you want to add some system dependencies
- Because you want to add some library dependencies to speed up tests
- Because you want to update the OS in the container
- etc.

## New languages

To add new languages you need to create a new base image. Builds for that
language will run in the new image. Of course, different services are free to
create their own build images, maybe using the image created here as a base.

To create a new base image, simply add a Dockerfile under a subdirectory of
`./base`, name the directory the same as the new language, for example
`./base/brainfuck`.

There are a few requirements for the base image you generate. To be safe, use
the `scala` image as a template for the python requirements you will need.

## Future work

- There is duplication between the different language images, they all need a
bunch of python dependencies. Extract that out to yet another meta-base image.
- Make it simpler to create build images that only add new library dependencies
in the target language. Different teams and services will need this.
- Provide a way to periodically refresh build images using the custom language
dependencies, as a way to speed up tests by not needing to download
dependencies.
