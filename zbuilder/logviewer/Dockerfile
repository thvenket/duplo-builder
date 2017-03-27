FROM ubuntu:14.04

RUN apt-get update
RUN apt-get install -y nano wget dialog net-tools vim curl
RUN apt-get install -y nginx
RUN rm -v /etc/nginx/nginx.conf
ADD nginx.conf /etc/nginx/
RUN rm /usr/share/nginx/html/index.html
ADD index.html /usr/share/nginx/html/index.html
EXPOSE 80

RUN apt-get install --yes --no-install-recommends supervisor
RUN mkdir -p /var/log/supervisor
ADD supervisord.conf /etc/supervisor/conf.d/supervisord.conf

EXPOSE 80
ENTRYPOINT ["/bin/bash", "-c", "/usr/bin/supervisord -c /etc/supervisor/conf.d/supervisord.conf -n"]