FROM ubuntu:14.04
RUN apt-get clean
RUN apt-get update
RUN apt-get install --yes --no-install-recommends software-properties-common
RUN apt-get install --yes --no-install-recommends curl
RUN apt-get install --yes --no-install-recommends wget
RUN apt-get install --yes --no-install-recommends build-essential python2.7 python2.7-dev python-pip
RUN apt-get install --yes --no-install-recommends libmysqlclient-dev
RUN apt-get install --yes --no-install-recommends --fix-missing libffi-dev
RUN apt-get install --yes --no-install-recommends libssl-dev
RUN apt-get install --yes --no-install-recommends libxml2-dev libxslt1-dev
RUN apt-get install --yes --no-install-recommends --fix-missing mysql-client
RUN apt-get install --yes libmysqlclient-dev
RUN apt-get install --yes mysql-server
RUN pip install --upgrade six>=1.10
RUN pip install boto==2.9.7
RUN echo "deb http://apt.dockerproject.org/repo ubuntu-trusty main" > /etc/apt/sources.list.d/docker.list
RUN apt-key adv --keyserver hkp://p80.pool.sks-keyservers.net:80 --recv-keys 58118E89F3A912897C070ADBF76221572C52609D
RUN apt-get update
RUN apt-get install --yes docker-engine=1.10.3-0~trusty
RUN apt-get install --yes --no-install-recommends supervisor
RUN apt-get install --yes --no-install-recommends python-dateutil
RUN apt-get install --yes --no-install-recommends vim

# Install node and Ember dependencies
RUN curl -sL https://deb.nodesource.com/setup_4.x | bash && \
        apt-get install --yes --no-install-recommends nodejs libfontconfig
RUN npm install -g bower \
    && sudo npm install -g phantomjs-prebuilt \
    && npm install -g ember-cli
RUN pip install -U pip
RUN pip install awscli
