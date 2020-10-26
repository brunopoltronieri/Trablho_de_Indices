import os
import random
import struct
import time
import tweepy

from datetime import datetime
from loguru import logger

TWEETS_FORMAT = '19s200s1120sdI'
RAW_DIR = 'eleicoes'
USER = 'bruno'
HASHTAGS_FILE = 'hashtags.txt'
REQUEST_COUNT = 1000

def read_tweets(api, hashtag):
    logger.debug(f'hashtag: {hashtag}')
    filename = os.path.join(RAW_DIR, f'{USER}_{datetime.utcnow().strftime("%m_%d_%Y__%H_%M_%S")}.raw')

    with open(filename, "wb") as raw_file:
        tweets = tweepy.Cursor(api.search, q=hashtag).items(REQUEST_COUNT)

        for tweet in tweets:
            rec = (
                tweet.id_str.encode('utf-8'),
                tweet.user.name.encode('utf-8'),
                tweet.text.encode('utf-8'),
                tweet.created_at.timestamp(),
                tweet.retweet_count
            )

            b = struct.pack(TWEETS_FORMAT, *rec)
            logger.debug(f'read tweet {tweet.id_str}')

            raw_file.write(b)
            logger.debug('wrote tweet to file')

    logger.debug(f'wrote {REQUEST_COUNT} tweets')

def read_hashtags():
    result = []
    
    with open(HASHTAGS_FILE, "r") as hashtags:
        for hashtag in hashtags:
            hashtag = hashtag.strip('\n')
            result.append(hashtag)

    return result

def main():
    try:
        hashtags = read_hashtags()
        auth = tweepy.OAuthHandler('b0if3VKn7rIsqqzkVwoiPUR56', 'QiLjZOJ86EgQZ4FIPaQvVKjcJg2x9AlePMv8oRPW4YMu7ZuNj1')
        auth.set_access_token('1176636173277679616-sXGihslJSCwCOfTT1p9DLiG1dH6rqp', 'N63e1CLlNfOPxsIIN5CzGuMZIhgcFDQkUYWlj2rUrQIhf')
        api = tweepy.API(auth)

        while True:
            random.shuffle(hashtags)
            logger.debug('shuffle')

            for hashtag in hashtags:
                read_tweets(api, hashtag)
                time.sleep(600)
    
    except KeyboardInterrupt:
        logger.debug("stopped executing")

if __name__ == "__main__":
    main()