FROM ubuntu:24
WORKDIR ~/wes

# Copy in the source code
COPY src ./src
EXPOSE 8000

# Setup an app user so the container doesn't run as the root user
RUN useradd $USER
USER $USER
