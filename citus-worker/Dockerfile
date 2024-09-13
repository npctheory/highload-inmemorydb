FROM citusdata/citus:latest
USER root

RUN apt-get update && \
    rm -rf /var/lib/apt/lists/*
    # apt-get install -y python3 python3-pip python3-venv procps && \

# RUN python3 -m venv /opt/venv && \
#     /opt/venv/bin/pip install --upgrade pip && \
#     /opt/venv/bin/pip install psycopg2-binary requests==2.31.0
# ENV PATH="/opt/venv/bin:$PATH"


COPY initdb /docker-entrypoint-initdb.d/
COPY docker-healthcheck /docker-healthcheck/

USER postgres
ENTRYPOINT ["docker-entrypoint.sh"]
EXPOSE 5432
CMD ["postgres"]
