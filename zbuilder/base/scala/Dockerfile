FROM ubuntu:14.04
RUN apt-get clean
RUN apt-get update
RUN apt-get install --yes --no-install-recommends software-properties-common
RUN apt-get install --yes --no-install-recommends curl
RUN apt-get install --yes --no-install-recommends wget
RUN apt-get install --yes --no-install-recommends build-essential python2.7 python2.7-dev python-pip
RUN echo "deb http://apt.dockerproject.org/repo ubuntu-trusty main" > /etc/apt/sources.list.d/docker.list
RUN apt-key adv --keyserver hkp://p80.pool.sks-keyservers.net:80 --recv-keys 58118E89F3A912897C070ADBF76221572C52609D
RUN apt-get update
RUN apt-get install --yes docker-engine=1.10.3-0~trusty
RUN apt-get install --yes --no-install-recommends supervisor
RUN apt-get install --yes --no-install-recommends vim
RUN apt-get install --yes --no-install-recommends apt-transport-https

#jdk repository
RUN add-apt-repository ppa:openjdk-r/ppa

#sbt repository
RUN echo "deb https://dl.bintray.com/sbt/debian /" | tee -a /etc/apt/sources.list.d/sbt.list
RUN echo "deb https://dl.bintray.com/sbt/debian /" | tee -a /etc/apt/sources.list.d/sbt.list
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv 642AC823

RUN apt-get update

RUN apt-get install --yes --no-install-recommends openjdk-8-jdk
RUN update-alternatives --config java
RUN update-alternatives --config javac
RUN apt-get install --yes --no-install-recommends sbt=0.13.13

# initialize sbt for faster runs
RUN sbt help
# faster dependency fetching using coursier
RUN mkdir -p ~/.sbt/0.13/plugins && echo 'addSbtPlugin("io.get-coursier" % "sbt-coursier" % "1.0.0-M15")' > ~/.sbt/0.13/plugins/build.sbt
# initialize sbt-coursier
RUN sbt help

# other stuff needed by the builder scripts
RUN pip install --upgrade six>=1.10
RUN pip install boto==2.9.7
RUN pip install -U pip
RUN pip install awscli
