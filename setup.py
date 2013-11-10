from setuptools import setup

def readme():
    with open('README.rst') as f:
        return f.read()

# silence nosetest 
try:
    import multiprocessing
except ImportError:
    pass
    
setup(name='kolada',
      version='0.1',
      description='python wrapper for the kolada-api',
      long_description=readme(),
      classifiers=[
        'Development Status :: 3 - Alpha',
        'License :: OSI Approved :: MIT License',
        'Programming Language :: Python :: 2.7'
      ],
      keywords='kolada',
      author='Ola Engwall',
      license='MIT',
      packages=['kolada'],
      install_requires=[
      ],
      test_suite='nose.collector',
      tests_require=['nose'],
      include_package_data=True)
