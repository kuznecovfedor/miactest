--
-- PostgreSQL database dump
--

-- Dumped from database version 14.9 (Ubuntu 14.9-0ubuntu0.22.04.1)
-- Dumped by pg_dump version 14.9 (Ubuntu 14.9-0ubuntu0.22.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

-- CREATE ROLE miac;
-- ALTER ROLE miac WITH NOSUPERUSER INHERIT NOCREATEROLE NOCREATEDB LOGIN NOREPLICATION NOBYPASSRLS PASSWORD 'SCRAM-SHA-256$4096:/xw6TeHd8iqRm+LON7LBaQ==$B3oRXVuM3xCf9Fx2GjPdcWj7+ORh+Y7kv+8EUWRwe+o=:I+zoBxe+EUv3pQ4jYA9pcYmKUW3BhcXj6sMlg3Ry4BA=';
--
-- Name: miac_test; Type: DATABASE; Schema: -; Owner: miac
--

-- CREATE DATABASE miac_test WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'en_US.UTF-8';

ALTER DATABASE miac_test OWNER TO miac;

\connect miac_test

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: material; Type: TABLE; Schema: public; Owner: miac
--

CREATE TABLE public.material (
    id_material integer NOT NULL,
    name character varying(100) NOT NULL,
    price numeric(12,2) DEFAULT 0.0 NOT NULL,
    id_seller integer NOT NULL,
    CONSTRAINT ch_price CHECK ((price >= (0)::numeric))
);


ALTER TABLE public.material OWNER TO miac;

--
-- Name: material_id_material_seq; Type: SEQUENCE; Schema: public; Owner: miac
--

ALTER TABLE public.material ALTER COLUMN id_material ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.material_id_material_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: seller; Type: TABLE; Schema: public; Owner: miac
--

CREATE TABLE public.seller (
    id_seller integer NOT NULL,
    name character varying(100) NOT NULL,
    surname character varying(100) NOT NULL,
    patronymic character varying(100),
    registration_date date DEFAULT CURRENT_DATE NOT NULL,
    login character varying(30) NOT NULL,
    password_hash character varying(60) NOT NULL
);


ALTER TABLE public.seller OWNER TO miac;

--
-- Name: seller_id_seller_seq; Type: SEQUENCE; Schema: public; Owner: miac
--

ALTER TABLE public.seller ALTER COLUMN id_seller ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.seller_id_seller_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: material pk_material; Type: CONSTRAINT; Schema: public; Owner: miac
--

ALTER TABLE ONLY public.material
    ADD CONSTRAINT pk_material PRIMARY KEY (id_material);


--
-- Name: seller pk_seller; Type: CONSTRAINT; Schema: public; Owner: miac
--

ALTER TABLE ONLY public.seller
    ADD CONSTRAINT pk_seller PRIMARY KEY (id_seller);


--
-- Name: material uq_id_seller_name; Type: CONSTRAINT; Schema: public; Owner: miac
--

ALTER TABLE ONLY public.material
    ADD CONSTRAINT uq_id_seller_name UNIQUE (id_seller, name);


--
-- Name: seller uq_login; Type: CONSTRAINT; Schema: public; Owner: miac
--

ALTER TABLE ONLY public.seller
    ADD CONSTRAINT uq_login UNIQUE (login);


--
-- Name: material fk_seller_material; Type: FK CONSTRAINT; Schema: public; Owner: miac
--

ALTER TABLE ONLY public.material
    ADD CONSTRAINT fk_seller_material FOREIGN KEY (id_seller) REFERENCES public.seller(id_seller) ON UPDATE CASCADE;


--
-- Name: DATABASE miac_test; Type: ACL; Schema: -; Owner: miac
--

REVOKE CONNECT,TEMPORARY ON DATABASE miac_test FROM PUBLIC;


--
-- Name: DEFAULT PRIVILEGES FOR TABLES; Type: DEFAULT ACL; Schema: -; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE miac GRANT ALL ON TABLES  TO miac;


--
-- PostgreSQL database dump complete
--

