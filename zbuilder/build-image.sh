#!/usr/bin/env bash

# Build zbuilder images for different languages.
# It uses the Dockerfiles found in subdirectories of ./base as base
# to build docker images that will run a build in Katkit.
#
# Usage:
#   ./build-image.sh python
#   ./build-image.sh scala
#   ./build-image.sh all
#
# That will create both the base image and the builder image. The resulting
# image will be named <OrgName>/zbuilder-$lang. For example zenefits/zbuilder-scala
# Passing all as argument builds all available languages.
#
# To add a new language, create a subdirectory in ./base and add a Dockerfile there.
# New base images must include all the necessary assets to run the build, that
# includes python, pip, awscli and some others. Use the scala example as a base
# for new languages.

set -o errexit
set -o pipefail
set -o nounset

function availableLangs() {
    (cd base && find . -name Dockerfile) | cut -d / -f 2
}

function usage() {
    echo "Usage"
    echo "  ${BASH_SOURCE[0]} lang"
    echo
    echo "Where lang is one of all,$(availableLangs | paste -d, -s)"
}

function buildLanguage() {
    local lang="${1}"
    local dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    local baseDockerfile="${dir}/base/${lang}/Dockerfile"

    if ! [ -f "${baseDockerfile}" ]
    then
        echo "File ${baseDockerfile} doesn't exist"
        echo
        usage
        exit 2
    fi

    local baseImageName="<OrgName>/zbuilder-${lang}-base"
    local builderImageName="<OrgName>/zbuilder-${lang}"

    echo "Building base image ${baseImageName} using ${baseDockerfile}"
    (cd "${dir}/base/${lang}" && docker build -t "${baseImageName}" .)

    local buildir=$(mktemp -dt "$(basename ${0}).XXXX")
    trap "rm -rf ${buildir}" EXIT

    echo "Building zbuilder image ${builderImageName} in directory ${buildir}"

    sed "s#__FROM__#${baseImageName}#" "${dir}/Dockerfile.template" > "${buildir}/Dockerfile"
    cp -r * "${buildir}"

    (cd ${buildir} && docker build -t "${builderImageName}" .)
}

lang="${1:-}"

if [ -z "${lang}" ]
then
    usage
    exit 1
fi

if [ "${lang}" = "all" ]
then
    for l in $(availableLangs); do
        "${0}" "${l}"
    done
else
    buildLanguage "${lang}"
fi
