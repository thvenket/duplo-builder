FROM ubuntu:14.04
RUN apt-get clean
RUN apt-get update
RUN apt-get install --yes --no-install-recommends software-properties-common
RUN apt-get install --yes --no-install-recommends curl
RUN apt-get install --yes --no-install-recommends wget
RUN apt-get install --yes --no-install-recommends build-essential python2.7 python2.7-dev python-pip
RUN apt-get install --yes --no-install-recommends apt-transport-https ca-certificates

# stack repository
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 575159689BEFB442
RUN echo 'deb http://download.fpcomplete.com/ubuntu trusty main'|tee /etc/apt/sources.list.d/fpco.list

# docker repository
RUN apt-key adv --keyserver hkp://p80.pool.sks-keyservers.net:80 --recv-keys 58118E89F3A912897C070ADBF76221572C52609D
RUN echo "deb http://apt.dockerproject.org/repo ubuntu-trusty main" > /etc/apt/sources.list.d/docker.list

RUN apt-get update
RUN apt-get install --yes stack
RUN apt-get install --yes docker-engine=1.10.3-0~trusty
RUN apt-get install --yes --no-install-recommends supervisor
RUN apt-get install --yes --no-install-recommends vim

# other stuff needed by the builder scripts
RUN pip install --upgrade six>=1.10
RUN pip install setuptools
RUN pip install boto==2.9.7
RUN pip install -U pip
RUN pip install awscli

# Pre install haskell compiler and base dependencies
RUN stack setup
